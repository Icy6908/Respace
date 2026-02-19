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
            if (!IsPostBack) { BindSupportGrid(); }
        }

        private void BindSupportGrid(string searchTerm = "")
        {
            string query = @"SELECT q.QueryID, q.Subject, q.Message, q.AdminReply, q.Status, u.FullName 
                             FROM SupportQueries q 
                             JOIN Users u ON q.UserId = u.UserId";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " WHERE (u.FullName LIKE @search OR q.Subject LIKE @search)";
                gvSupport.DataSource = Db.Query(query, new SqlParameter("@search", "%" + searchTerm + "%"));
            }
            else
            {
                gvSupport.DataSource = Db.Query(query);
            }
            gvSupport.DataBind();
        }

        protected void txtSearchSupport_TextChanged(object sender, EventArgs e)
        {
            BindSupportGrid(txtSearchSupport.Text.Trim());
        }

        protected void gvSupport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SubmitReply")
            {
                try
                {
                    string queryId = e.CommandArgument.ToString();
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    TextBox txtReply = (TextBox)row.FindControl("txtReply");

                    // Server-side validation (Security/Bug Check)
                    if (txtReply == null || string.IsNullOrWhiteSpace(txtReply.Text))
                    {
                        // Fallback if client-side validation fails
                        return;
                    }

                    string replyText = txtReply.Text.Trim();

                    // SQL Injection protection via parameters is already used
                    string sql = "UPDATE SupportQueries SET AdminReply = @reply, Status = 'Resolved' WHERE QueryID = @id";

                    Db.Query(sql,
                        new SqlParameter("@reply", replyText),
                        new SqlParameter("@id", queryId));

                    // Refresh the list - the 'Resolved' status will now hide the textbox automatically
                    BindSupportGrid(txtSearchSupport.Text.Trim());

                    // Optional: Success message
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Reply sent successfully!');", true);


                }
                catch (Exception ex)
                {
                    // Bug Check: Log the error and show user-friendly message
                    // LogError(ex); 
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('A database error occurred. Please try again later.');", true);
                }
            }
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSupportQueries();
        }

        private void LoadSupportQueries()
        {
            string selectedStatus = ddlStatusFilter.SelectedValue;
            string search = txtSearchSupport.Text.Trim();

            // Base Query joining with Users to get FullName
            string query = @"SELECT q.*, u.FullName 
                     FROM SupportQueries q 
                     JOIN Users u ON q.UserId = u.UserId 
                     WHERE 1=1";

            List<SqlParameter> paramList = new List<SqlParameter>();

            // Apply Status Filter
            if (selectedStatus != "All")
            {
                query += " AND q.Status = @status";
                paramList.Add(new SqlParameter("@status", selectedStatus));
            }

            // Apply Search Filter
            if (!string.IsNullOrEmpty(search))
            {
                query += " AND (u.FullName LIKE @search OR q.Subject LIKE @search)";
                paramList.Add(new SqlParameter("@search", "%" + search + "%"));
            }

            // FIXED: Changed 'CreatedAt' to 'SubmittedAt' to match your database schema
            query += " ORDER BY q.SubmittedAt DESC";

            gvSupport.DataSource = Db.Query(query, paramList.ToArray());
            gvSupport.DataBind();
        }

        public string GetSupportStatusClass(string status)
        {
            switch (status?.Trim())
            {
                case "Resolved": return "bg-success text-white";
                case "Pending": return "bg-warning text-dark";
                default: return "bg-secondary text-white";
            }
        }
    }
}