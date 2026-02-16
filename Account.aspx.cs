using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class Account : System.Web.UI.Page
    {
        private int UserId => (Session["UserId"] == null) ? 0 : Convert.ToInt32(Session["UserId"]);
        private string Role => (Session["Role"] ?? "").ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ApplyRoleUI();
                LoadUserHeader(Role == "Guest");
                if (Role == "Guest") LoadMembershipLabel();
                else lblMembership.Text = "";

                LoadSummary(Role == "Guest", Role == "Host");

                // default tab
                if (Role == "Host") SetActiveTab(2);
                else if (Role == "Guest") SetActiveTab(1);
                else SetActiveTab(0);
            }
        }

        private void ApplyRoleUI()
        {
            bool isGuest = Role == "Guest";
            bool isHost = Role == "Host";

            phGuestTabs.Visible = isGuest;
            phHostTabs.Visible = isHost;

            phGuestSummary.Visible = isGuest;
            phHostSummary.Visible = isHost;

            phHostActions.Visible = isHost;
            phGuestActions.Visible = isGuest;

            phGuestPointsUI.Visible = isGuest;
            phHostEarningsUI.Visible = isHost;

            phMembershipUI.Visible = isGuest;
        }

        private void LoadUserHeader(bool loadPoints)
        {
            DataTable dt = Db.Query(@"
                SELECT FullName, Email, Role, PointsBalance
                FROM Users
                WHERE UserId=@Id
            ", new SqlParameter("@Id", UserId));

            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];
            lblName.Text = r["FullName"].ToString();
            lblEmail.Text = r["Email"].ToString();
            lblRole.Text = r["Role"].ToString();

            if (loadPoints)
                lblPoints.Text = r["PointsBalance"].ToString();
        }

        private void LoadMembershipLabel()
        {
            try
            {
                object planName = Db.Scalar(@"
                    SELECT TOP 1 p.PlanName
                    FROM UserMemberships um
                    INNER JOIN MembershipPlans p ON p.PlanId = um.PlanId
                    WHERE um.UserId=@UserId AND um.IsActive=1
                    ORDER BY um.StartDate DESC
                ", new SqlParameter("@UserId", UserId));

                lblMembership.Text = (planName == null) ? "Free" : planName.ToString();
            }
            catch
            {
                lblMembership.Text = "Free";
            }
        }

        private void LoadSummary(bool isGuest, bool isHost)
        {
            if (isGuest)
            {
                object totalBookings = Db.Scalar(
                    "SELECT COUNT(*) FROM Bookings WHERE GuestUserId=@U",
                    new SqlParameter("@U", UserId));

                object upcomingBookings = Db.Scalar(@"
                    SELECT COUNT(*) FROM Bookings
                    WHERE GuestUserId=@U
                      AND LTRIM(RTRIM(Status))='Confirmed'
                      AND StartDateTime >= GETDATE()
                ", new SqlParameter("@U", UserId));

                lblTotalBookings.Text = Convert.ToInt32(totalBookings).ToString();
                lblUpcomingBookings.Text = Convert.ToInt32(upcomingBookings).ToString();
            }

            if (isHost)
            {
                // ✅ include Draft too (host wants to see drafts in overview)
                object activeListings = Db.Scalar(@"
                    SELECT COUNT(*) FROM Spaces
                    WHERE HostUserId=@U
                      AND IsDeleted=0
                      AND Status IN ('Draft','Approved','Pending')
                ", new SqlParameter("@U", UserId));

                lblActiveListings.Text = Convert.ToInt32(activeListings).ToString();

                object earningsObj = Db.Scalar(@"
                    SELECT ISNULL(SUM(b.TotalPrice), 0)
                    FROM Bookings b
                    INNER JOIN Spaces s ON s.SpaceId=b.SpaceId
                    WHERE s.HostUserId=@U
                      AND s.IsDeleted=0
                      AND LTRIM(RTRIM(b.Status))='Confirmed'
                ", new SqlParameter("@U", UserId));

                decimal earnings = Convert.ToDecimal(earningsObj);
                lblEarnings.Text = earnings.ToString("C");
                lblEarnings2.Text = earnings.ToString("C");
            }
        }

        private void LoadGuestBookingsGrid()
        {
            DataTable dt = Db.Query(@"
                SELECT b.BookingId,
                       s.Name AS SpaceName,
                       b.StartDateTime,
                       b.EndDateTime,
                       b.TotalPrice,
                       LTRIM(RTRIM(b.Status)) AS Status
                FROM Bookings b
                INNER JOIN Spaces s ON s.SpaceId=b.SpaceId
                WHERE b.GuestUserId=@U
                ORDER BY b.StartDateTime DESC
            ", new SqlParameter("@U", UserId));

            gvBookings.DataSource = dt;
            gvBookings.DataBind();
            lblBookingsMsg.Text = dt.Rows.Count == 0 ? "No bookings yet." : "";
        }

        private void LoadHostSpacesGrid()
        {
            // ✅ show Draft/Pending/Approved for host, still hide deleted
            DataTable dt = Db.Query(@"
                SELECT SpaceId, Name, Location, Type, PricePerHour, Status
                FROM Spaces
                WHERE HostUserId=@U
                  AND IsDeleted=0
                  AND Status IN ('Draft','Pending','Approved')
                ORDER BY CreatedAt DESC
            ", new SqlParameter("@U", UserId));

            gvSpaces.DataSource = dt;
            gvSpaces.DataBind();
            lblSpacesMsg.Text = dt.Rows.Count == 0 ? "No listings yet." : "";
        }

        private void LoadHostBookingsGrid()
        {
            // ✅ exclude deleted spaces so host bookings list stays clean
            DataTable dt = Db.Query(@"
                SELECT b.BookingId,
                       s.Name AS SpaceName,
                       u.FullName AS GuestName,
                       b.StartDateTime,
                       b.EndDateTime,
                       b.TotalPrice,
                       LTRIM(RTRIM(b.Status)) AS Status
                FROM Bookings b
                INNER JOIN Spaces s ON s.SpaceId=b.SpaceId
                INNER JOIN Users u ON u.UserId=b.GuestUserId
                WHERE s.HostUserId=@U
                  AND s.IsDeleted=0
                ORDER BY b.StartDateTime DESC
            ", new SqlParameter("@U", UserId));

            gvHostBookings.DataSource = dt;
            gvHostBookings.DataBind();
            lblHostBookingsMsg.Text = dt.Rows.Count == 0 ? "No bookings yet." : "";
        }

        private void SetActiveTab(int viewIndex)
        {
            bool isGuest = Role == "Guest";
            bool isHost = Role == "Host";

            if (viewIndex == 1 && !isGuest) viewIndex = 0;
            if (viewIndex == 2 && !isHost) viewIndex = 0;

            mv.ActiveViewIndex = viewIndex;

            btnTabOverview.CssClass = "tab";
            if (isGuest) btnTabBookings.CssClass = "tab";
            if (isHost) btnTabSpaces.CssClass = "tab";

            if (viewIndex == 0) btnTabOverview.CssClass = "tab active";
            if (viewIndex == 1 && isGuest) btnTabBookings.CssClass = "tab active";
            if (viewIndex == 2 && isHost) btnTabSpaces.CssClass = "tab active";

            if (viewIndex == 1 && isGuest) LoadGuestBookingsGrid();
            if (viewIndex == 2 && isHost)
            {
                LoadHostSpacesGrid();
                LoadHostBookingsGrid();
            }
        }

        protected void btnTabOverview_Click(object sender, EventArgs e) => SetActiveTab(0);
        protected void btnTabBookings_Click(object sender, EventArgs e) => SetActiveTab(1);
        protected void btnTabSpaces_Click(object sender, EventArgs e) => SetActiveTab(2);

        // HOST: Edit / BlockDates / Delete listing
        protected void gvSpaces_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (Role != "Host") { SetActiveTab(0); return; }

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int spaceId))
                return;

            if (e.CommandName == "EditSpace")
            {
                Response.Redirect("HostCreateSpace.aspx?id=" + spaceId);
                return;
            }

            // ✅ open block dates page
            if (e.CommandName == "BlockDates")
            {
                Response.Redirect("HostBlockDates.aspx?spaceId=" + spaceId);
                return;
            }

            if (e.CommandName == "DeleteSpace")
            {
                int affected = Db.Execute(@"
                    UPDATE dbo.Spaces
                    SET IsDeleted = 1
                    WHERE SpaceId=@Id AND HostUserId=@U AND IsDeleted=0
                ",
                new SqlParameter("@Id", spaceId),
                new SqlParameter("@U", UserId));

                lblActionMsg.Text = affected > 0
                    ? "<div class='alert success'>Listing hidden.</div>"
                    : "<div class='alert error'>Delete failed (not found / not yours).</div>";

                ApplyRoleUI();
                LoadUserHeader(false);
                LoadSummary(false, true);
                SetActiveTab(2);
            }
        }

        // HOST: Confirm/Reject/Cancel booking
        protected void gvHostBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (Role != "Host") { SetActiveTab(0); return; }

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int bookingId))
                return;

            int affected = 0;

            if (e.CommandName == "HostConfirmBooking")
            {
                affected = Db.Execute(@"
                    UPDATE b
                    SET b.Status='Confirmed'
                    FROM Bookings b
                    INNER JOIN Spaces s ON s.SpaceId=b.SpaceId
                    WHERE b.BookingId=@B
                      AND s.HostUserId=@U
                      AND LTRIM(RTRIM(b.Status))='Pending'
                ",
                new SqlParameter("@B", bookingId),
                new SqlParameter("@U", UserId));

                lblActionMsg.Text = affected > 0
                    ? "<div class='alert success'>Booking confirmed.</div>"
                    : "<div class='alert error'>Confirm failed (not Pending anymore).</div>";
            }
            else if (e.CommandName == "HostRejectBooking")
            {
                affected = Db.Execute(@"
                    UPDATE b
                    SET b.Status='Cancelled'
                    FROM Bookings b
                    INNER JOIN Spaces s ON s.SpaceId=b.SpaceId
                    WHERE b.BookingId=@B
                      AND s.HostUserId=@U
                      AND LTRIM(RTRIM(b.Status))='Pending'
                ",
                new SqlParameter("@B", bookingId),
                new SqlParameter("@U", UserId));

                lblActionMsg.Text = affected > 0
                    ? "<div class='alert success'>Booking rejected.</div>"
                    : "<div class='alert error'>Reject failed.</div>";
            }
            else if (e.CommandName == "HostCancelBooking")
            {
                affected = Db.Execute(@"
                    UPDATE b
                    SET b.Status='Cancelled'
                    FROM Bookings b
                    INNER JOIN Spaces s ON s.SpaceId=b.SpaceId
                    WHERE b.BookingId=@B
                      AND s.HostUserId=@U
                      AND LTRIM(RTRIM(b.Status)) <> 'Cancelled'
                      AND b.StartDateTime > GETDATE()
                ",
                new SqlParameter("@B", bookingId),
                new SqlParameter("@U", UserId));

                lblActionMsg.Text = affected > 0
                    ? "<div class='alert success'>Booking cancelled.</div>"
                    : "<div class='alert error'>Cancel failed (maybe already started).</div>";
            }

            ApplyRoleUI();
            LoadUserHeader(false);
            LoadSummary(false, true);
            SetActiveTab(2);
        }

        // GUEST: Cancel pending booking
        protected void gvBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (Role != "Guest") { SetActiveTab(0); return; }

            if (e.CommandName != "GuestCancel") return;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int bookingId))
                return;

            int affected = Db.Execute(@"
                UPDATE Bookings
                SET Status='Cancelled'
                WHERE BookingId=@B
                  AND GuestUserId=@U
                  AND LTRIM(RTRIM(Status))='Pending'
                  AND StartDateTime > GETDATE()
            ",
            new SqlParameter("@B", bookingId),
            new SqlParameter("@U", UserId));

            lblActionMsg.Text = affected > 0
                ? "<div class='alert success'>Booking cancelled.</div>"
                : "<div class='alert error'>Cancel failed (maybe confirmed/started).</div>";

            ApplyRoleUI();
            LoadUserHeader(true);
            LoadSummary(true, false);
            SetActiveTab(1);
        }
    }
}
