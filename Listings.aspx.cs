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
    public partial class Listings : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings[DbConfig.ConnectionStringName].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadListings();
            }
        }

        private void LoadListings(string categoryFilter = "All")
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // THE TRANSLATOR: We select THEIR columns but rename them to OUR names using 'AS'
                // This means your GridView Eval("Title") will ALWAYS work.
                string query = $@"SELECT 
                         L.{DbConfig.ListingId} AS ListingID, 
                         L.{DbConfig.ListingTitle} AS Title, 
                         ISNULL(U.{DbConfig.UserName}, 'No Owner') AS OwnerName, 
                         L.{DbConfig.ListingCategory} AS Category, 
                         L.{DbConfig.ListingPrice} AS Price, 
                         L.{DbConfig.ListingStatus} AS Status 
                         FROM {DbConfig.ListingTable} L 
                         LEFT JOIN {DbConfig.UserTable} U ON L.{DbConfig.ListingOwnerId} = U.{DbConfig.UserId}";

                if (categoryFilter != "All")
                {
                    query += $" WHERE L.{DbConfig.ListingCategory} LIKE '%' + @category + '%'";
                }

                SqlCommand cmd = new SqlCommand(query, con);

                if (categoryFilter != "All")
                {
                    cmd.Parameters.AddWithValue("@category", categoryFilter.Trim());
                }

                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvListings.DataSource = dt;
                    gvListings.DataBind();
                }
                catch (Exception ex)
                {
                    // This prevents the whole site from crashing if a column name is wrong
                    System.Diagnostics.Debug.WriteLine("Integration Error: " + ex.Message);
                }
            }
        }

        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadListings(ddlCategoryFilter.SelectedValue);
        }

        protected void gvListings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int listingId = Convert.ToInt32(e.CommandArgument);
            string sql = "";

            // Use constants from DbConfig to build the Update/Delete queries
            if (e.CommandName == "ApproveListing")
            {
                sql = $"UPDATE {DbConfig.ListingTable} SET {DbConfig.ListingStatus} = 'Approved' WHERE {DbConfig.ListingId} = @id";
            }
            else if (e.CommandName == "RejectListing")
            {
                sql = $"UPDATE {DbConfig.ListingTable} SET {DbConfig.ListingStatus} = 'Rejected' WHERE {DbConfig.ListingId} = @id";
            }
            else if (e.CommandName == "DeleteListing")
            {
                sql = $"DELETE FROM {DbConfig.ListingTable} WHERE {DbConfig.ListingId} = @id";
            }

            if (!string.IsNullOrEmpty(sql))
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@id", listingId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadListings(ddlCategoryFilter.SelectedValue);
            }
        }
    }
}