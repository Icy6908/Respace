using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class Bookings : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings[DbConfig.ConnectionStringName].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBookings();
            }
        }

        private void LoadBookings()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // SQL ALIASING: We select THEIR column name but call it OUR name (BookingID, etc.)
                // This prevents "Column not found" errors in the GridView later.
                string sql = $@"SELECT 
                                {DbConfig.BookingId} AS BookingID, 
                                {DbConfig.BookingListingId} AS ListingID, 
                                {DbConfig.BookingRenterId} AS RenterID, 
                                {DbConfig.BookingDate} AS BookingDate, 
                                {DbConfig.BookingStatus} AS Status 
                                FROM {DbConfig.BookingTable} 
                                ORDER BY {DbConfig.BookingDate} DESC";

                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                gvBookings.DataSource = cmd.ExecuteReader();
                gvBookings.DataBind();
            }
        }

        protected void gvBookings_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // GridView uses the aliased name 'BookingID' as the DataKey
            int bookingId = Convert.ToInt32(gvBookings.DataKeys[e.RowIndex].Value);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                // Versatile DELETE using DbConfig table and ID names
                string sql = $"DELETE FROM {DbConfig.BookingTable} WHERE {DbConfig.BookingId} = @id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", bookingId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadBookings();
        }
    }
}