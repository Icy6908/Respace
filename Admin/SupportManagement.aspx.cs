using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace.Admin
{
    public partial class SupportManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSupportQueries();
            }
        }

        protected void txtSearchSupport_TextChanged(object sender, EventArgs e) => LoadSupportQueries();

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e) => LoadSupportQueries();

        private void LoadSupportQueries()
        {
            string selectedStatus = ddlStatusFilter.SelectedValue;
            string search = txtSearchSupport.Text.Trim();

            // Unified SQL Query pulling all necessary fields
            string query = @"SELECT q.*, u.FullName 
                             FROM SupportQueries q 
                             JOIN Users u ON q.UserId = u.UserId 
                             WHERE 1=1";

            List<SqlParameter> paramList = new List<SqlParameter>();

            if (selectedStatus != "All")
            {
                query += " AND q.Status = @status";
                paramList.Add(new SqlParameter("@status", selectedStatus));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND (u.FullName LIKE @search OR q.Subject LIKE @search OR CAST(q.QueryID AS NVARCHAR) LIKE @search)";
                paramList.Add(new SqlParameter("@search", "%" + search + "%"));
            }

            query += " ORDER BY q.SubmittedAt DESC";

            gvSupport.DataSource = Db.Query(query, paramList.ToArray());
            gvSupport.DataBind();
        }

        protected void gvSupport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string queryId = e.CommandArgument.ToString();

            if (e.CommandName == "SubmitReply")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                TextBox txtReply = (TextBox)row.FindControl("txtReply");

                if (txtReply != null && !string.IsNullOrWhiteSpace(txtReply.Text))
                {
                    string sql = "UPDATE SupportQueries SET AdminReply = @reply, Status = 'Resolved' WHERE QueryID = @id";
                    Db.Query(sql, new SqlParameter("@reply", txtReply.Text.Trim()), new SqlParameter("@id", queryId));

                    LoadSupportQueries();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Reply sent and status updated to Resolved!');", true);
                }
            }
            else if (e.CommandName == "DeleteTicket")
            {
                string sql = "DELETE FROM SupportQueries WHERE QueryID = @id";
                Db.Query(sql, new SqlParameter("@id", queryId));

                LoadSupportQueries();
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Ticket # " + queryId + " has been permanently deleted.');", true);
            }
        }

        public string GetSupportStatusClass(string status)
        {
            switch (status?.Trim())
            {
                case "Resolved": return "bg-success";
                case "Pending": return "bg-warning text-dark";
                default: return "bg-secondary";
            }
        }
    }
}