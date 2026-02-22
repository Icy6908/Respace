using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Respace
{
    public partial class BookingSuccess : System.Web.UI.Page
    {
        private int UserId => Convert.ToInt32(Session["UserId"]);

        private int BookingId
        {
            get
            {
                int id;
                int.TryParse(Request.QueryString["bookingId"], out id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (BookingId <= 0)
                {
                    lblMsg.Text = "Booking created. View it in your account.";
                    pnlDetails.Visible = false;
                    return;
                }

                LoadBooking();
            }
        }

        private void LoadBooking()
        {
          
            DataTable dt = Db.Query(@"
                SELECT 
                    b.BookingId,
                    b.StartDateTime,
                    b.EndDateTime,
                    b.TotalPrice,
                    b.Status,
                    s.Name AS SpaceName,
                    s.Location
                FROM Bookings b
                INNER JOIN Spaces s ON s.SpaceId = b.SpaceId
                WHERE b.BookingId=@B AND b.GuestUserId=@U
            ",
            new SqlParameter("@B", BookingId),
            new SqlParameter("@U", UserId));

            if (dt.Rows.Count == 0)
            {
                lblMsg.Text = "Booking not found (or not yours).";
                pnlDetails.Visible = false;
                return;
            }

            var r = dt.Rows[0];

            DateTime checkIn = Convert.ToDateTime(r["StartDateTime"]).Date;
            DateTime checkOut = Convert.ToDateTime(r["EndDateTime"]).Date;
            int nights = (checkOut - checkIn).Days;
            if (nights < 0) nights = 0;

            lblSpaceName.Text = r["SpaceName"].ToString();
            lblLocation.Text = r["Location"].ToString();
            lblCheckIn.Text = checkIn.ToString("dd MMM yyyy");
            lblCheckOut.Text = checkOut.ToString("dd MMM yyyy");
            lblNights.Text = nights.ToString();
            lblTotal.Text = Convert.ToDecimal(r["TotalPrice"]).ToString("C");
            lblStatus.Text = r["Status"].ToString();

            pnlDetails.Visible = true;
        }

        protected void btnViewBookings_Click(object sender, EventArgs e)
        {
            Response.Redirect("Account.aspx?tab=bookings");
        }
    }
}
