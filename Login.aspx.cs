using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var email = txtEmail.Text.Trim().ToLower();
            var hash = Security.Sha256(txtPassword.Text);

            DataTable dt = Db.Query(@"SELECT UserId, FullName, Role, PointsBalance
                                      FROM Users
                                      WHERE Email=@Email AND PasswordHash=@Hash",
                new SqlParameter("@Email", email),
                new SqlParameter("@Hash", hash));

            if (dt.Rows.Count == 0)
            {
                lblMsg.Text = "Invalid email/password.";
                return;
            }

            Session["UserId"] = dt.Rows[0]["UserId"];
            Session["FullName"] = dt.Rows[0]["FullName"];
            Session["Role"] = dt.Rows[0]["Role"];
            Session["PointsBalance"] = dt.Rows[0]["PointsBalance"];

            var role = Session["Role"].ToString();
            if (role == "Admin") Response.Redirect("AdminApproveSpaces.aspx");
            else if (role == "Host") Response.Redirect("HostMySpaces.aspx");
            else Response.Redirect("Search.aspx");
        }
    }
}
