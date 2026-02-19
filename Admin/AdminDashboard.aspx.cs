using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace.Admin
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        // Public variables for JavaScript injection
        public string CategoryLabels = "[]";
        public string CategoryData = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Security Bypass / Session Check
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("../Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindStats();
                BindCharts();
            }
        }

        private void BindStats()
        {
            try
            {
                // --- EXISTING BASIC STATS ---
                DataTable dtUsers = Db.Query("SELECT COUNT(*) FROM Users WHERE Role != 'Admin'");
                int totalUsers = Convert.ToInt32(dtUsers.Rows[0][0]);
                lblTotalUsers.Text = totalUsers.ToString();

                DataTable dtSpaces = Db.Query("SELECT COUNT(*) FROM Spaces WHERE Status = 'Pending'");
                lblPendingSpaces.Text = dtSpaces.Rows[0][0].ToString();

                DataTable dtRev = Db.Query("SELECT SUM(Amount) FROM Payments WHERE Status = 'Completed'");
                decimal revenueAmount = dtRev.Rows[0][0] != DBNull.Value ? Convert.ToDecimal(dtRev.Rows[0][0]) : 0;
                lblTotalRevenue.Text = revenueAmount.ToString("N2");
                lblNetCommission.Text = (revenueAmount * 0.10m).ToString("N2");

                DataTable dtBookings = Db.Query("SELECT COUNT(*) FROM Bookings WHERE Status = 'Confirmed'");
                int confirmedCount = Convert.ToInt32(dtBookings.Rows[0][0]);
                lblConfirmedBookings.Text = confirmedCount.ToString();

                // --- NEW KPI 1: Average Order Value (AOV) ---
                // Formula: Total Revenue / Number of Bookings
                decimal aov = confirmedCount > 0 ? (revenueAmount / confirmedCount) : 0;
                lblAOV.Text = aov.ToString("N2");

                // --- NEW KPI 2: Utilization Rate ---
                // Formula: Confirmed Bookings / Total Active Spaces (Simplified for MVP)
                DataTable dtActiveSpaces = Db.Query("SELECT COUNT(*) FROM Spaces WHERE Status = 'Active'");
                int activeSpaceCount = Convert.ToInt32(dtActiveSpaces.Rows[0][0]);
                double utilization = activeSpaceCount > 0 ? ((double)confirmedCount / activeSpaceCount) * 100 : 0;
                lblUtilization.Text = string.Format("{0:0.0}%", utilization);

                // --- NEW KPI 3: User Churn (Inactive Users) ---
                // Logic: Users who haven't logged in for 30 days (assuming you have a 'LastLogin' column)
                // If you don't have LastLogin, we check users with 0 bookings
                DataTable dtInactive = Db.Query("SELECT COUNT(*) FROM Users WHERE UserId NOT IN (SELECT GuestUserId FROM Bookings)");
                int inactiveCount = Convert.ToInt32(dtInactive.Rows[0][0]);
                double churn = totalUsers > 0 ? ((double)inactiveCount / totalUsers) * 100 : 0;
                lblChurnRate.Text = string.Format("{0:0.0}%", churn);

                // --- RATIO & SUPPORT ---
                DataTable dtHosts = Db.Query("SELECT COUNT(*) FROM Users WHERE Role = 'Host'");
                DataTable dtGuests = Db.Query("SELECT COUNT(*) FROM Users WHERE Role = 'Guest'");
                lblUserRatio.Text = dtHosts.Rows[0][0].ToString() + ":" + dtGuests.Rows[0][0].ToString();

                DataTable dtQueries = Db.Query("SELECT COUNT(*) FROM SupportQueries WHERE Status = 'Pending'");
                lblOpenQueries.Text = dtQueries.Rows[0][0].ToString();
            }
            catch (Exception ex) { /* Log Error */ }
        }

        private void BindCharts()
        {
            // Fetch Category Data from Spaces table
            DataTable dt = Db.Query("SELECT Category, COUNT(*) as Total FROM Spaces GROUP BY Category");

            List<string> labels = new List<string>();
            List<int> counts = new List<int>();

            foreach (DataRow row in dt.Rows)
            {
                labels.Add(row["Category"].ToString());
                counts.Add(Convert.ToInt32(row["Total"]));
            }

            // Convert to JSON-compatible strings
            CategoryLabels = "['" + string.Join("','", labels) + "']";
            CategoryData = "[" + string.Join(",", counts) + "]";
        }
    }
}