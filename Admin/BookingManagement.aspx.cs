using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace.Admin
{
    public partial class BookingManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindBookingGrid();
            }
        }

        private void BindBookingGrid(string searchTerm = "")
        {
            
            string query = @"SELECT b.BookingId, s.Name as SpaceName, u.FullName as RenterName, 
                             b.StartDateTime, b.TotalPrice, b.Status 
                             FROM Bookings b
                             JOIN Spaces s ON b.SpaceId = s.SpaceId
                             JOIN Users u ON b.GuestUserId = u.UserId";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " WHERE s.Name LIKE @search OR u.FullName LIKE @search";
                DataTable dt = Db.Query(query, new SqlParameter("@search", "%" + searchTerm + "%"));
                gvBookings.DataSource = dt;
            }
            else
            {
                DataTable dt = Db.Query(query);
                gvBookings.DataSource = dt;
            }
            gvBookings.DataBind();
        }

        protected void txtSearchBooking_TextChanged(object sender, EventArgs e)
        {
            BindBookingGrid(txtSearchBooking.Text.Trim());
        }

       
        public string GetBookingStatusClass(string status)
        {
            switch (status?.Trim())
            {
                case "Confirmed": return "bg-success";
                case "Pending": return "bg-warning text-dark";
                case "Cancelled": return "bg-danger";
                default: return "bg-secondary";
            }
        }
    }
}