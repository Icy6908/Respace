using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

namespace Respace.Admin
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        // ViewState properties for JS charts
        public string CategoryLabels = "[]";
        public string CategoryData = "[]";
        public string RevenueTypeLabels = "[]";
        public string RevenueTypeData = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
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
                // USER METRICS
                DataTable dtUsers = Db.Query("SELECT COUNT(*) FROM Users WHERE Role != 'Admin'");
                int totalUsers = Convert.ToInt32(dtUsers.Rows[0][0]);
                lblTotalUsers.Text = totalUsers.ToString();

                DataTable dtHosts = Db.Query("SELECT COUNT(*) FROM Users WHERE Role = 'Host'");
                DataTable dtGuests = Db.Query("SELECT COUNT(*) FROM Users WHERE Role = 'Guest'");
                lblUserRatio.Text = dtHosts.Rows[0][0].ToString() + ":" + dtGuests.Rows[0][0].ToString();

                // FINANCIALS & AOV
                DataTable dtRev = Db.Query("SELECT SUM(Amount) FROM Payments WHERE Status = 'Completed'");
                decimal revenueAmount = dtRev.Rows[0][0] != DBNull.Value ? Convert.ToDecimal(dtRev.Rows[0][0]) : 0;
                lblTotalRevenue.Text = revenueAmount.ToString("N2");
                lblNetCommission.Text = (revenueAmount * 0.10m).ToString("N2");

                DataTable dtBookings = Db.Query("SELECT COUNT(*) FROM Bookings WHERE Status = 'Confirmed'");
                int confirmedCount = Convert.ToInt32(dtBookings.Rows[0][0]);
                lblConfirmedBookings.Text = confirmedCount.ToString();

                decimal aov = confirmedCount > 0 ? (revenueAmount / confirmedCount) : 0;
                lblAOV.Text = aov.ToString("N2");

                // SPACE UTILIZATION & CHURN
                DataTable dtActiveSpaces = Db.Query("SELECT COUNT(*) FROM Spaces WHERE Status = 'Active'");
                int activeSpaceCount = Convert.ToInt32(dtActiveSpaces.Rows[0][0]);
                double utilization = activeSpaceCount > 0 ? ((double)confirmedCount / activeSpaceCount) * 100 : 0;
                lblUtilization.Text = string.Format("{0:0.0}%", utilization);

                DataTable dtInactive = Db.Query("SELECT COUNT(*) FROM Users WHERE UserId NOT IN (SELECT GuestUserId FROM Bookings) AND Role != 'Admin'");
                int inactiveCount = Convert.ToInt32(dtInactive.Rows[0][0]);
                double churn = totalUsers > 0 ? ((double)inactiveCount / totalUsers) * 100 : 0;
                lblChurnRate.Text = string.Format("{0:0.0}%", churn);

                // APPROVALS & SUPPORT
                DataTable dtPending = Db.Query("SELECT COUNT(*) FROM Spaces WHERE Status = 'Pending'");
                lblPendingSpaces.Text = dtPending.Rows[0][0].ToString();

                DataTable dtOpen = Db.Query("SELECT COUNT(*) FROM SupportQueries WHERE Status = 'Pending'");
                lblOpenQueries.Text = dtOpen.Rows[0][0].ToString();
            }
            catch { /* Quietly handle errors */ }
        }

        private void BindCharts()
        {
            try
            {
                // 1. Space Mix by Category (Existing)
                DataTable dtCat = Db.Query("SELECT Category, COUNT(*) as Total FROM Spaces GROUP BY Category");
                List<string> labels = new List<string>();
                List<int> counts = new List<int>();
                foreach (DataRow row in dtCat.Rows)
                {
                    labels.Add(row["Category"].ToString());
                    counts.Add(Convert.ToInt32(row["Total"]));
                }
                CategoryLabels = "['" + string.Join("','", labels) + "']";
                CategoryData = "[" + string.Join(",", counts) + "]";

                // 2. NEW: Gross Revenue by Space Type
                // Calculates total before commission
                DataTable dtRevType = Db.Query(@"
                    SELECT s.Type, SUM(b.TotalPrice) as GrossTotal 
                    FROM Bookings b 
                    JOIN Spaces s ON b.SpaceId = s.SpaceId 
                    WHERE b.Status = 'Confirmed' 
                    GROUP BY s.Type");

                List<string> revLabels = new List<string>();
                List<decimal> revTotals = new List<decimal>();
                foreach (DataRow row in dtRevType.Rows)
                {
                    revLabels.Add(row["Type"].ToString());
                    revTotals.Add(Convert.ToDecimal(row["GrossTotal"]));
                }
                RevenueTypeLabels = "['" + string.Join("','", revLabels) + "']";
                RevenueTypeData = "[" + string.Join(",", revTotals) + "]";
            }
            catch { /* Quietly handle errors */ }
        }
    }
}