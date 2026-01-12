using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class Users : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings[DbConfig.ConnectionStringName].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // VERSATILE QUERY: Translates THEIR database names into YOUR GridView names
                string query = $@"SELECT 
                    {DbConfig.UserId} AS UserID, 
                    {DbConfig.UserName} AS FullName, 
                    {DbConfig.UserEmail} AS Email, 
                    {DbConfig.UserType} AS UserType, 
                    {DbConfig.UserStatus} AS Status, 
                    {DbConfig.UserIsApproved} AS IsApproved 
                    FROM {DbConfig.UserTable}";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUsers.DataSource = dt;
                gvUsers.DataBind();
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewHistory")
            {
                Response.Redirect("UserHistory.aspx?UserId=" + e.CommandArgument);
                return;
            }

            int userId = Convert.ToInt32(e.CommandArgument);
            string sql = "";

            // Use DbConfig constants to build the logic
            switch (e.CommandName)
            {
                case "ApproveUser":
                    sql = $"UPDATE {DbConfig.UserTable} SET {DbConfig.UserIsApproved} = 1, {DbConfig.UserStatus} = 'Active' WHERE {DbConfig.UserId} = @id";
                    break;
                case "SuspendUser":
                    sql = $"UPDATE {DbConfig.UserTable} SET {DbConfig.UserStatus} = 'Suspended' WHERE {DbConfig.UserId} = @id";
                    break;
                case "ActivateUser":
                    sql = $"UPDATE {DbConfig.UserTable} SET {DbConfig.UserStatus} = 'Active' WHERE {DbConfig.UserId} = @id";
                    break;
                case "DeleteUser":
                    sql = $"DELETE FROM {DbConfig.UserTable} WHERE {DbConfig.UserId} = @id";
                    break;
            }

            if (!string.IsNullOrEmpty(sql))
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@id", userId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadUsers();
            }
        }
    }
}