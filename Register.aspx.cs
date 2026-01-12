using System;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class Register : System.Web.UI.Page
    {
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();
            var email = txtEmail.Text.Trim().ToLower();
            var pw = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pw))
            {
                lblMsg.Text = "Please fill in all fields.";
                return;
            }

            var exists = Db.Scalar("SELECT COUNT(*) FROM Users WHERE Email=@Email",
                new SqlParameter("@Email", email));

            if (Convert.ToInt32(exists) > 0)
            {
                lblMsg.Text = "Email already registered.";
                return;
            }

            var hash = Security.Sha256(pw);
            var role = ddlRole.SelectedValue;

            Db.Execute(@"INSERT INTO Users(FullName, Email, PasswordHash, Role)
                         VALUES(@Name, @Email, @Hash, @Role)",
                new SqlParameter("@Name", name),
                new SqlParameter("@Email", email),
                new SqlParameter("@Hash", hash),
                new SqlParameter("@Role", role));

            Response.Redirect("Login.aspx");
        }
    }
}
