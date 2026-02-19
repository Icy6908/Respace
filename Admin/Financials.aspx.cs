using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;

namespace Respace.Admin
{
    public partial class Financials : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Default to showing current month
                txtStartDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
                txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                BindFinancials();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindFinancials();
        }

        private DataTable GetFinancialData()
        {
            List<SqlParameter> paramList = new List<SqlParameter>();
            string query = @"SELECT p.PaymentID, p.Amount, p.Status, p.PaymentDate, 
                             s.Name as SpaceName, u.FullName as RenterName
                             FROM Payments p
                             JOIN Bookings b ON p.BookingId = b.BookingId
                             JOIN Spaces s ON b.SpaceId = s.SpaceId
                             JOIN Users u ON b.GuestUserId = u.UserId
                             WHERE 1=1";

            if (!string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
            {
                query += " AND p.PaymentDate >= @start AND p.PaymentDate <= @end";
                paramList.Add(new SqlParameter("@start", txtStartDate.Text));
                paramList.Add(new SqlParameter("@end", txtEndDate.Text + " 23:59:59"));
            }

            query += " ORDER BY p.PaymentDate DESC";
            return Db.Query(query, paramList.ToArray());
        }

        private void BindFinancials()
        {
            DataTable dt = GetFinancialData();
            gvFinancials.DataSource = dt;
            gvFinancials.DataBind();

            decimal totalReceived = 0;
            foreach (DataRow row in dt.Rows)
            {
                string status = row["Status"].ToString();
                if (status == "Completed" || status == "Success")
                {
                    totalReceived += Convert.ToDecimal(row["Amount"]);
                }
            }

            // dashboard calculations
            decimal hostPayout = totalReceived * 0.90m;
            decimal commission = totalReceived * 0.10m;

            litTotalReceived.Text = totalReceived.ToString("C");
            litHostPayout.Text = hostPayout.ToString("C");
            litCommission.Text = commission.ToString("C");
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = GetFinancialData();
            StringBuilder sb = new StringBuilder();

            // CSV Header
            sb.AppendLine("Ref ID,Space,Paid By,Date,Total Paid,Host Payout (90%),Commission (10%),Status");

            foreach (DataRow row in dt.Rows)
            {
                decimal total = Convert.ToDecimal(row["Amount"]);
                sb.AppendLine(string.Format("{0},{1},{2},{3:yyyy-MM-dd},{4:F2},{5:F2},{6:F2},{7}",
                    row["PaymentID"],
                    row["SpaceName"].ToString().Replace(",", ""),
                    row["RenterName"].ToString().Replace(",", ""),
                    row["PaymentDate"],
                    total,
                    total * 0.90m,
                    total * 0.10m,
                    row["Status"]
                ));
            }

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Respace_Financials.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        public string GetPaymentStatusClass(string status)
        {
            switch (status?.Trim())
            {
                case "Completed":
                case "Success": return "bg-success";
                case "Pending": return "bg-warning text-dark";
                case "Failed": return "bg-danger";
                default: return "bg-secondary";
            }
        }
    }
}