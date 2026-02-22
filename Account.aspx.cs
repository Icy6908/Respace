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
            if (UserId == 0) { Response.Redirect("Login.aspx"); return; }


            gvSpaces.RowDataBound += gvSpaces_RowDataBound;

            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] != null)
                {
                    lblActionMsg.Text = GetMessageText(Request.QueryString["msg"]);
                    lblActionMsg.Visible = true;
                    lblActionMsg.CssClass = "alert success";
                }

                ApplyRoleUI();
                LoadUserHeader();
                if (Role == "Guest") LoadMembershipLabel();
                LoadSummary();

                if (Role == "Host") SetActiveTab(2);
                else if (Role == "Guest") SetActiveTab(1);
                else SetActiveTab(0);
            }
        }

        private string GetMessageText(string msgCode)
        {
            switch (msgCode)
            {
                case "MembershipActivated": return "Your membership has been upgraded successfully!";
                case "BookingConfirmed": return "Booking paid and confirmed!";
                case "PasswordChanged": return "Your password has been updated successfully.";
                default: return "";
            }
        }

        private void ApplyRoleUI()
        {
            bool isG = (Role == "Guest");
            bool isH = (Role == "Host");
            phGuestTabs.Visible = phGuestSummary.Visible = phGuestActions.Visible = phGuestPointsUI.Visible = phMembershipUI.Visible = isG;
            phHostTabs.Visible = phHostSummary.Visible = phHostActions.Visible = phHostEarningsUI.Visible = isH;
        }

        private void LoadUserHeader()
        {
            DataTable dt = Db.Query("SELECT FullName, Email, Role, PointsBalance FROM Users WHERE UserId=@Id", new SqlParameter("@Id", UserId));
            if (dt.Rows.Count > 0)
            {
                lblName.Text = dt.Rows[0]["FullName"].ToString();
                lblEmail.Text = dt.Rows[0]["Email"].ToString();
                lblRole.Text = dt.Rows[0]["Role"].ToString();
                if (Role == "Guest") lblPoints.Text = dt.Rows[0]["PointsBalance"].ToString();
            }
        }

        private void LoadMembershipLabel()
        {
            object plan = Db.Scalar(@"SELECT TOP 1 p.PlanName FROM UserMemberships um 
                                     INNER JOIN MembershipPlans p ON p.PlanId = um.PlanId 
                                     WHERE um.UserId=@U AND um.IsActive=1", new SqlParameter("@U", UserId));
            lblMembership.Text = plan?.ToString() ?? "Free";
        }

        private void LoadSummary()
        {
            if (Role == "Guest")
            {
                lblUpcomingBookings.Text = Db.Scalar("SELECT COUNT(*) FROM Bookings WHERE GuestUserId=@U AND Status='Confirmed' AND StartDateTime > GETDATE()", new SqlParameter("@U", UserId)).ToString();
                lblTotalBookings.Text = Db.Scalar("SELECT COUNT(*) FROM Bookings WHERE GuestUserId=@U", new SqlParameter("@U", UserId)).ToString();
            }
            else if (Role == "Host")
            {
                lblActiveListings.Text = Db.Scalar("SELECT COUNT(*) FROM Spaces WHERE HostUserId=@U AND IsDeleted=0", new SqlParameter("@U", UserId)).ToString();
                object total = Db.Scalar(@"SELECT SUM(TotalPrice) FROM Bookings b JOIN Spaces s ON s.SpaceId = b.SpaceId 
                                          WHERE s.HostUserId=@U AND b.Status='Confirmed'", new SqlParameter("@U", UserId));
                lblEarnings.Text = lblEarnings2.Text = string.Format("{0:C}", total == DBNull.Value ? 0 : total);
            }
        }

        private void SetActiveTab(int index)
        {
            mv.ActiveViewIndex = index;
            btnTabOverview.CssClass = (index == 0) ? "tab active" : "tab";
            if (Role == "Guest")
            {
                btnTabBookings.CssClass = (index == 1) ? "tab active" : "tab";
                if (index == 1) LoadGuestBookingsGrid();
            }
            if (Role == "Host")
            {
                btnTabSpaces.CssClass = (index == 2) ? "tab active" : "tab";
                if (index == 2) { LoadHostSpacesGrid(); LoadHostBookingsGrid(); }
            }
        }

 

        protected void gvSpaces_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (Control c in e.Row.Cells[e.Row.Cells.Count - 1].Controls)
                {
                    if (c is LinkButton btn && btn.CommandName == "DeleteSpace")
                    {
                        
                        btn.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this listing? This action cannot be undone.');");
                    }
                }
            }
        }

        protected void gvSpaces_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "EditSpace") Response.Redirect("HostCreateSpace.aspx?id=" + id);
            if (e.CommandName == "BlockDates") Response.Redirect("HostBlockDates.aspx?spaceId=" + id);

            if (e.CommandName == "DeleteSpace")
            {
             
                Db.Execute("UPDATE Spaces SET IsDeleted=1 WHERE SpaceId=@Id", new SqlParameter("@Id", id));

                LoadHostSpacesGrid();
                LoadSummary();

                lblActionMsg.Text = "Listing deleted successfully.";
                lblActionMsg.CssClass = "alert success";
                lblActionMsg.Visible = true;
            }
        }

        protected void gvBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (Role != "Guest") return;
            int bid = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "GuestCancel")
            {
                Db.Execute(@"UPDATE Bookings SET Status='Cancelled' WHERE BookingId=@B AND GuestUserId=@U 
                            AND (Status='Pending' OR Status='Confirmed')", new SqlParameter("@B", bid), new SqlParameter("@U", UserId));
                LoadGuestBookingsGrid();
                lblActionMsg.Text = "Booking cancelled successfully.";
                lblActionMsg.CssClass = "alert success";
                lblActionMsg.Visible = true;
            }
            else if (e.CommandName == "CompleteStay")
            {
                Response.Redirect($"Review.aspx?bid={bid}");
            }
        }

        protected void gvHostBookings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int bid = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "HostConfirmBooking")
            {
                Db.Execute("UPDATE Bookings SET Status='Confirmed' WHERE BookingId=@B", new SqlParameter("@B", bid));
            }
            else if (e.CommandName == "HostCancelBooking")
            {
                Db.Execute("UPDATE Bookings SET Status='Cancelled' WHERE BookingId=@B", new SqlParameter("@B", bid));
            }
            LoadHostBookingsGrid();
        }

        private void LoadGuestBookingsGrid()
        {
            gvBookings.DataSource = Db.Query(@"SELECT b.BookingId, s.Name as SpaceName, b.StartDateTime, b.EndDateTime, b.TotalPrice, b.Status 
                                              FROM Bookings b JOIN Spaces s ON s.SpaceId=b.SpaceId 
                                              WHERE b.GuestUserId=@U ORDER BY b.BookingId DESC", new SqlParameter("@U", UserId));
            gvBookings.DataBind();
        }

        private void LoadHostSpacesGrid()
        {
            gvSpaces.DataSource = Db.Query("SELECT SpaceId, Name, Status FROM Spaces WHERE HostUserId=@U AND IsDeleted=0", new SqlParameter("@U", UserId));
            gvSpaces.DataBind();
        }

        private void LoadHostBookingsGrid()
        {
            gvHostBookings.DataSource = Db.Query(@"SELECT b.BookingId, s.Name as SpaceName, u.FullName as GuestName, b.Status 
                                                  FROM Bookings b JOIN Spaces s ON s.SpaceId=b.SpaceId 
                                                  JOIN Users u ON u.UserId=b.GuestUserId 
                                                  WHERE s.HostUserId=@U ORDER BY b.BookingId DESC", new SqlParameter("@U", UserId));
            gvHostBookings.DataBind();
        }

        protected void btnTabOverview_Click(object sender, EventArgs e) => SetActiveTab(0);
        protected void btnTabBookings_Click(object sender, EventArgs e) => SetActiveTab(1);
        protected void btnTabSpaces_Click(object sender, EventArgs e) => SetActiveTab(2);

        protected void btnCancelMembership_Click(object sender, EventArgs e)
        {
            Db.Execute("UPDATE UserMemberships SET IsActive = 0 WHERE UserId = @uid", new SqlParameter("@uid", UserId));
            LoadMembershipLabel();
            lblActionMsg.Text = "Your membership has been cancelled.";
            lblActionMsg.Visible = true;
        }
    }
}