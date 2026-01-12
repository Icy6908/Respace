using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class GuestMyBookings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Guest")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack) LoadBookings();
        }

        private void LoadBookings()
        {
            int guestUserId = Convert.ToInt32(Session["UserId"]);

            DataTable dt = Db.Query(@"
                SELECT b.BookingId, s.Name, b.StartDateTime, b.EndDateTime, b.TotalPrice, b.Status
                FROM Bookings b
                INNER JOIN Spaces s ON s.SpaceId = b.SpaceId
                WHERE b.GuestUserId=@GuestUserId
                ORDER BY b.StartDateTime DESC
            ", new SqlParameter("@GuestUserId", guestUserId));

            gv.DataSource = dt;
            gv.DataBind();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "CancelBooking") return;

            int bookingId = Convert.ToInt32(e.CommandArgument);
            int guestUserId = Convert.ToInt32(Session["UserId"]);

            // find the reason textbox in the clicked row
            var btn = (Control)e.CommandSource;
            var row = (GridViewRow)btn.NamingContainer;
            var txtReason = (TextBox)row.FindControl("txtReason");
            string reason = txtReason?.Text?.Trim();

            int rows = Db.Execute(@"
                UPDATE Bookings
                SET Status='Cancelled',
                    CancelComment=@Reason,
                    CancelledBy='Guest',
                    CancelledAt=GETDATE()
                WHERE BookingId=@BookingId
                  AND GuestUserId=@GuestUserId
                  AND Status='Confirmed'
            ",
            new SqlParameter("@Reason", (object)reason ?? DBNull.Value),
            new SqlParameter("@BookingId", bookingId),
            new SqlParameter("@GuestUserId", guestUserId));

            lblMsg.ForeColor = rows > 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMsg.Text = rows > 0 ? "Booking cancelled." : "Unable to cancel (maybe already cancelled).";
            LoadBookings();
        }
    }
}
