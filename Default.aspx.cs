using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Respace.App_Code;

namespace Respace
{
    public partial class Default : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings[DbConfig.ConnectionStringName].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardStats();
            }
        }

        private void LoadDashboardStats()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    //Count Total Users 
                    string sqlUsers = $"SELECT COUNT(*) FROM {DbConfig.UserTable}";
                    SqlCommand cmdUsers = new SqlCommand(sqlUsers, con);
                    lblTotalUsers.Text = cmdUsers.ExecuteScalar().ToString();

                    //Count Pending Listings 
                    // use the status column name from DbConfig
                    string sqlPending = $"SELECT COUNT(*) FROM {DbConfig.ListingTable} WHERE {DbConfig.ListingStatus} = 'Pending'";
                    SqlCommand cmdPending = new SqlCommand(sqlPending, con);
                    lblPendingSpaces.Text = cmdPending.ExecuteScalar().ToString();

                    //Sum Total Payment
                    string sqlRev = $"SELECT SUM({DbConfig.PaymentAmount}) FROM {DbConfig.PaymentTable} WHERE {DbConfig.PaymentStatus} = 'Completed'";
                    SqlCommand cmdRev = new SqlCommand(sqlRev, con);
                    object rev = cmdRev.ExecuteScalar();
                    lblRevenue.Text = (rev != DBNull.Value) ? string.Format("{0:C}", rev) : "$0.00";

                    // 4. Count Open Support Queries (Versatile)
                    string sqlSupport = $"SELECT COUNT(*) FROM {DbConfig.SupportTable} WHERE {DbConfig.SupportStatus} = 'Open'";
                    SqlCommand cmdSupport = new SqlCommand(sqlSupport, con);
                    lblOpenQueries.Text = cmdSupport.ExecuteScalar().ToString();
                }
            }
            catch (SqlException ex)
            {
                //Log it instead of just writing to screen
                lblTotalUsers.Text = "Error";
                lblPendingSpaces.Text = "Error";
                //keep the message for debugging integration
                System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
            }
        }
    }
}