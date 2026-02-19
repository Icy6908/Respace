using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
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
            var confirm = txtConfirm.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pw) || string.IsNullOrWhiteSpace(confirm))
            {
                lblMsg.Text = "Please fill in all fields.";
                return;
            }

            if (!Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {
                lblMsg.Text = "Please enter a valid email address.";
                return;
            }

            if (pw.Length < 8)
            {
                lblMsg.Text = "Password must be at least 8 characters.";
                return;
            }

            if (!string.Equals(pw, confirm))
            {
                lblMsg.Text = "Passwords do not match.";
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
            
            lblMsg.Text = "Registration successful!";
            lblMsg.Visible = true;

        }
    }
}
