using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Respace.App_Code;

namespace Respace
{
    public partial class HostBookings : System.Web.UI.Page
    {
        private int SpaceId
        {
            get { int.TryParse(Request.QueryString["spaceId"], out int id); return id; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"]?.ToString() != "Host")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (SpaceId <= 0)
            {
                lblMsg.Text = "Invalid space.";
                return;
            }

            if (!IsPostBack) LoadBookings();
        }

        private bool HostOwnsSpace(int hostUserId)
        {
            object v = Db.Scalar("SELECT COUNT(*) FROM Spaces WHERE SpaceId=@Id AND HostUserId=@Host",
                new SqlParameter("@Id", SpaceId),
                new SqlParameter("@Host", hostUserId));

            return Convert.ToInt32(v) > 0;
        }

        private void LoadBookings()
        {
            int hostUserId = Convert.ToInt32(Session["UserId"]);
            if (!HostOwnsSpace(hostUserId))
            {
                lblMsg.Text = "You don't own this space.";
                gv.Visible = false;
                return;
            }

            DataTable dt = Db.Query(@"
                SELECT b.BookingId, u.FullName AS GuestName, b.StartDateTime, b.EndDateTime, b.TotalPrice, b.Status
                FROM Bookings b
                INNER JOIN Users u ON u.UserId = b.GuestUserId
                WHERE b.SpaceId=@SpaceId
                ORDER BY b.StartDateTime DESC
            ", new SqlParameter("@SpaceId", SpaceId));

            gv.DataSource = dt;
            gv.DataBind();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "CancelBooking") return;

            int hostUserId = Convert.ToInt32(Session["UserId"]);
            if (!HostOwnsSpace(hostUserId))
            {
                lblMsg.Text = "Unauthorized.";
                return;
            }

            int bookingId = Convert.ToInt32(e.CommandArgument);

            var btn = (Control)e.CommandSource;
            var row = (GridViewRow)btn.NamingContainer;
            var txtReason = (TextBox)row.FindControl("txtReason");
            string reason = txtReason?.Text?.Trim();

            if (string.IsNullOrWhiteSpace(reason))
            {
                lblMsg.Text = "Please provide a reason.";
                return;
            }

            int rows = Db.Execute(@"
                UPDATE Bookings
                SET Status='Cancelled',
                    CancelComment=@Reason,
                    CancelledBy='Host',
                    CancelledAt=GETDATE()
                WHERE BookingId=@BookingId
                  AND SpaceId=@SpaceId
                  AND Status='Confirmed'
            ",
            new SqlParameter("@Reason", reason),
            new SqlParameter("@BookingId", bookingId),
            new SqlParameter("@SpaceId", SpaceId));

            lblMsg.ForeColor = rows > 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMsg.Text = rows > 0 ? "Booking cancelled." : "Unable to cancel.";
            LoadBookings();
        }
    }
}
