using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class UserHistory : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings[DbConfig.ConnectionStringName].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string userId = Request.QueryString["UserId"];
                if (!string.IsNullOrEmpty(userId))
                {
                    lblUserId.Text = userId;
                    LoadHistory(userId);
                }
            }
        }

        private void LoadHistory(string userId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // 1. Load Listings (Owned by this user)
                // Use Aliasing so even if DB has 'fld_title', UI sees 'Title'
                string listQuery = $@"SELECT 
                    {DbConfig.ListingId} AS ListingID, 
                    {DbConfig.ListingTitle} AS Title, 
                    {DbConfig.ListingCategory} AS Category, 
                    {DbConfig.ListingPrice} AS PricePerDay, 
                    {DbConfig.ListingStatus} AS Status 
                    FROM {DbConfig.ListingTable} 
                    WHERE {DbConfig.ListingOwnerId} = @id";

                SqlDataAdapter daList = new SqlDataAdapter(listQuery, con);
                daList.SelectCommand.Parameters.AddWithValue("@id", userId);
                DataTable dtList = new DataTable();
                daList.Fill(dtList);
                gvUserListings.DataSource = dtList;
                gvUserListings.DataBind();

                // 2. Load Bookings (Made by this user as a Renter)
                try
                {
                    string bookQuery = $@"SELECT 
                        {DbConfig.BookingId} AS BookingID, 
                        {DbConfig.BookingListingId} AS ListingID, 
                        {DbConfig.BookingDate} AS BookingDate, 
                        {DbConfig.BookingTotal} AS TotalPrice, 
                        {DbConfig.BookingStatus} AS Status 
                        FROM {DbConfig.BookingTable} 
                        WHERE {DbConfig.BookingRenterId} = @id";

                    SqlDataAdapter daBook = new SqlDataAdapter(bookQuery, con);
                    daBook.SelectCommand.Parameters.AddWithValue("@id", userId);
                    DataTable dtBook = new DataTable();
                    daBook.Fill(dtBook);
                    gvUserBookings.DataSource = dtBook;
                    gvUserBookings.DataBind();
                }
                catch (Exception ex)
                {
                    // Log error to debug console instead of crashing
                    System.Diagnostics.Debug.WriteLine("Booking Load Error: " + ex.Message);
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Users.aspx");
        }
    }
}