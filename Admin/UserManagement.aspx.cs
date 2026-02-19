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

        private void BindUserGrid(string searchTerm = "")
        {
            string query = "SELECT UserId, FullName, Email, Role, Status FROM Users";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " WHERE FullName LIKE @search OR Email LIKE @search";
                parameters.Add(new SqlParameter("@search", "%" + searchTerm + "%"));
            }

            gvUsers.DataSource = Db.Query(query, parameters.ToArray());
            gvUsers.DataBind();
        }

        protected void txtSearchUsers_TextChanged(object sender, EventArgs e)
        {
            BindUserGrid(txtSearchUsers.Text.Trim());
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string userId = e.CommandArgument.ToString();

            // The 'ViewUser' command now just redirects to the new page
            if (e.CommandName == "ViewUser")
            {
                Response.Redirect("UserView.aspx?id=" + userId);
            }
            else if (e.CommandName == "SuspendUser")
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