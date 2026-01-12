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
    public partial class Support : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings[DbConfig.ConnectionStringName].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSupportQueries();
            }
        }

        private void LoadSupportQueries()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // ALIASING: Maps their column names to the names used in your Repeater Eval()
                string sql = $@"SELECT 
                    {DbConfig.SupportId} AS QueryID, 
                    {DbConfig.SupportSubject} AS Subject, 
                    {DbConfig.SupportMessage} AS Issue, 
                    {DbConfig.SupportStatus} AS Status,
                    {DbConfig.SupportDate} AS SubmittedAt
                    FROM {DbConfig.SupportTable} 
                    ORDER BY {DbConfig.SupportDate} DESC";

                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                rptTickets.DataSource = cmd.ExecuteReader();
                rptTickets.DataBind();
            }
        }

        protected void rptTickets_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectTicket")
            {
                int ticketId = Convert.ToInt32(e.CommandArgument);
                LoadTicketDetails(ticketId);
            }
        }

        private void LoadTicketDetails(int ticketId)
        {
            pnlNoSelection.Visible = false;
            pnlMessageDetail.Visible = true;
            ViewState["SelectedTicketId"] = ticketId;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                // Adding a LEFT JOIN to Users in case you want to show the sender's real name later
                string query = $@"SELECT 
                    {DbConfig.SupportId} AS QueryID, 
                    {DbConfig.SupportSubject} AS Subject, 
                    {DbConfig.SupportMessage} AS Issue, 
                    {DbConfig.SupportStatus} AS Status
                    FROM {DbConfig.SupportTable} 
                    WHERE {DbConfig.SupportId} = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", ticketId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblSubject.Text = dr["Subject"].ToString();
                    litMessageContent.Text = dr["Issue"].ToString();
                    lblUserEmail.Text = "Ticket Reference #" + dr["QueryID"].ToString();
                }
            }
        }

        protected void btnStatus_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedTicketId"] != null)
            {
                Button btn = (Button)sender;
                string newStatus = btn.ID == "btnResolve" ? "Resolved" : "In-Progress";
                int ticketId = (int)ViewState["SelectedTicketId"];

                UpdateTicketStatus(ticketId, newStatus);
            }
        }

        private void UpdateTicketStatus(int ticketId, string status)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string sql = $"UPDATE {DbConfig.SupportTable} SET {DbConfig.SupportStatus} = @status WHERE {DbConfig.SupportId} = @id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@id", ticketId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            LoadSupportQueries();
            LoadTicketDetails(ticketId);
        }

        protected string GetStatusClass(string status)
        {
            switch (status)
            {
                case "Open": return "bg-danger";
                case "In-Progress": return "bg-primary";
                case "Resolved": return "bg-success text-white";
                default: return "bg-secondary";
            }
        }
    }
}