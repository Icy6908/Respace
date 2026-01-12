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
    public partial class Payments : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings[DbConfig.ConnectionStringName].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { LoadPayments(); LoadFinanceKPIs(); }
        }

        private void LoadPayments()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // VERSATILE QUERY: Maps DB names to UI names (Amount, Status, etc.)
                string sql = $@"SELECT 
                    {DbConfig.PaymentId} AS PaymentID, 
                    {DbConfig.PaymentBookingId} AS BookingID, 
                    {DbConfig.PaymentAmount} AS Amount, 
                    {DbConfig.PaymentType} AS PaymentType, 
                    {DbConfig.PaymentMethod} AS Gateway, 
                    {DbConfig.PaymentStatus} AS Status 
                    FROM {DbConfig.PaymentTable} 
                    ORDER BY {DbConfig.PaymentDate} DESC";

                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                gvPayments.DataSource = cmd.ExecuteReader();
                gvPayments.DataBind();
            }
        }

        private void LoadFinanceKPIs()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // VERSATILE MATH: Calculates Net Revenue using DbConfig constants
                string sqlIngoing = $@"
                    SELECT 
                    (SELECT ISNULL(SUM({DbConfig.PaymentAmount}), 0) FROM {DbConfig.PaymentTable} WHERE {DbConfig.PaymentStatus} = 'Completed') - 
                    (SELECT ISNULL(SUM({DbConfig.PaymentAmount}), 0) FROM {DbConfig.PaymentTable} WHERE {DbConfig.PaymentStatus} = 'Refunded')";

                SqlCommand cmdIngoing = new SqlCommand(sqlIngoing, con);
                object ingoingResult = cmdIngoing.ExecuteScalar();
                lblTotalIngoing.Text = string.Format("{0:C}", ingoingResult ?? 0);

                // PENDING PAYOUTS: Sums anything not finished or refunded
                string sqlPending = $"SELECT SUM({DbConfig.PaymentAmount}) FROM {DbConfig.PaymentTable} WHERE {DbConfig.PaymentStatus} NOT IN ('Completed', 'Refunded')";
                SqlCommand cmdPending = new SqlCommand(sqlPending, con);
                object pendingResult = cmdPending.ExecuteScalar();
                lblPendingPayouts.Text = string.Format("{0:C}", (pendingResult != DBNull.Value) ? pendingResult : 0);
            }
        }

        protected void gvPayments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int paymentId = Convert.ToInt32(e.CommandArgument);
            string sql = "";

            if (e.CommandName == "CompletePayment")
            {
                sql = $"UPDATE {DbConfig.PaymentTable} SET {DbConfig.PaymentStatus} = 'Completed' WHERE {DbConfig.PaymentId} = @id";
            }
            else if (e.CommandName == "IssueRefund")
            {
                sql = $"UPDATE {DbConfig.PaymentTable} SET {DbConfig.PaymentStatus} = 'Refunded' WHERE {DbConfig.PaymentId} = @id";
            }

            if (!string.IsNullOrEmpty(sql))
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@id", paymentId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadPayments();
                LoadFinanceKPIs();
            }
        }
    }
}