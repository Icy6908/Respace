using System;
using System.Collections.Generic;
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

        private void BindBookingGrid()
        {
            string searchTerm = txtSearchBooking.Text.Trim();
            string selectedStatus = ddlStatusFilter.SelectedValue;

            // Base query with table joins
            string query = @"SELECT b.BookingId, s.Name as SpaceName, u.FullName as RenterName, 
                             b.StartDateTime, b.TotalPrice, b.Status 
                             FROM Bookings b
                             JOIN Spaces s ON b.SpaceId = s.SpaceId
                             JOIN Users u ON b.GuestUserId = u.UserId
                             WHERE 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            // 1. Search Logic
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (s.Name LIKE @search OR u.FullName LIKE @search)";
                parameters.Add(new SqlParameter("@search", "%" + searchTerm + "%"));
            }

            // 2. Status Filter Logic
            if (selectedStatus != "All")
            {
                query += " AND b.Status = @status";
                parameters.Add(new SqlParameter("@status", selectedStatus));
            }

            query += " ORDER BY b.BookingId DESC";

            // Execute using the shared Db helper
            gvBookings.DataSource = Db.Query(query, parameters.ToArray());
            gvBookings.DataBind();
        }

        protected void txtSearchBooking_TextChanged(object sender, EventArgs e)
        {
            BindBookingGrid();
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindBookingGrid();
        }

        public string GetBookingStatusClass(string status)
        {
            switch (status?.Trim())
            {
                case "Confirmed": return "bg-success";
                case "Pending": return "bg-warning text-dark";
                case "Cancelled": return "bg-danger";
                case "Refunded": return "bg-secondary text-white"; // Added for your refund system
                default: return "bg-secondary";
            }
        }
    }
}