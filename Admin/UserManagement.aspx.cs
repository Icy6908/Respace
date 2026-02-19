using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Respace.App_Code;
using System.Collections.Generic;

namespace Respace.Admin
{
    public partial class UserManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUserGrid();
            }
        }

        // Combined filtering logic
        private void BindUserGrid()
        {
            string searchTerm = txtSearchUsers.Text.Trim();
            string selectedRole = ddlRoleFilter.SelectedValue;

            string query = "SELECT UserId, FullName, Email, Role, Status FROM Users WHERE 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            // 1. Role Filter Logic
            if (selectedRole != "All")
            {
                query += " AND Role = @role";
                parameters.Add(new SqlParameter("@role", selectedRole));
            }

            // 2. Search Term Logic
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (FullName LIKE @search OR Email LIKE @search)";
                parameters.Add(new SqlParameter("@search", "%" + searchTerm + "%"));
            }

            query += " ORDER BY UserId DESC";

            gvUsers.DataSource = Db.Query(query, parameters.ToArray());
            gvUsers.DataBind();
        }

        protected void txtSearchUsers_TextChanged(object sender, EventArgs e)
        {
            BindUserGrid();
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindUserGrid();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string userId = e.CommandArgument.ToString();

            if (e.CommandName == "SuspendUser")
            {
                Db.Query("UPDATE Users SET Status = 'Suspended' WHERE UserId = @id", new SqlParameter("@id", userId));
                BindUserGrid();
            }
            else if (e.CommandName == "RemoveUser")
            {
                Db.Query("DELETE FROM Users WHERE UserId = @id", new SqlParameter("@id", userId));
                BindUserGrid();
            }
        }

        public string GetStatusClass(string status)
        {
            switch (status?.Trim())
            {
                case "Active": return "bg-success";
                case "Suspended": return "bg-warning text-dark";
                case "Inactive": return "bg-secondary";
                default: return "bg-danger";
            }
        }
    }
}