using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
        }

        protected void btnSendOtp_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            int userId = Convert.ToInt32(Session["UserId"]);

            string current = txtCurrent.Text ?? "";
            if (current.Length < 8)
            {
                lblMsg.Text = "Enter your current password first.";
                return;
            }

            // Verify current password
            string hash = Security.Sha256(current);
            object ok = Db.Scalar("SELECT COUNT(1) FROM Users WHERE UserId=@U AND PasswordHash=@H",
                new SqlParameter("@U", userId),
                new SqlParameter("@H", hash));

            if (Convert.ToInt32(ok) != 1)
            {
                lblMsg.Text = "Current password is incorrect.";
                return;
            }

            // Get email + name
            DataTable dt = Db.Query("SELECT FullName, Email FROM Users WHERE UserId=@U",
                new SqlParameter("@U", userId));
            if (dt.Rows.Count == 0) return;

            string name = dt.Rows[0]["FullName"].ToString();
            string email = dt.Rows[0]["Email"].ToString();

            if (!OtpService.CanSend(userId, OtpService.PurposeChangePassword, out int wait))
            {
                lblMsg.Text = $"Please wait {wait}s before requesting another OTP.";
                return;
            }

            DateTime exp;
            string otp = OtpService.CreateAndStoreOtp(userId, OtpService.PurposeChangePassword, out exp);

            string subject = "ReSpace OTP - Change Password";
            string body = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5'>
                  <h2 style='margin:0 0 8px'>Change your password</h2>
                  <p>Hi {name},</p>
                  <p>Your OTP is:</p>
                  <div style='font-size:28px;font-weight:900;letter-spacing:4px'>{otp}</div>
                  <p style='color:#666'>This OTP expires at {exp:HH:mm}.</p>
                </div>";

            EmailService.Send(email, subject, body);

            lblMsg.Text = "OTP sent to your email.";
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            int userId = Convert.ToInt32(Session["UserId"]);

            string current = txtCurrent.Text ?? "";
            string newPw = txtNewPw.Text ?? "";
            string confirm = txtConfirm.Text ?? "";
            string otp = (txtOtp.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(current) || current.Length < 8)
            {
                lblMsg.Text = "Current password required.";
                return;
            }
            if (string.IsNullOrWhiteSpace(newPw) || newPw.Length < 8)
            {
                lblMsg.Text = "New password must be at least 8 characters.";
                return;
            }
            if (newPw != confirm)
            {
                lblMsg.Text = "Passwords do not match.";
                return;
            }
            if (otp.Length != 6)
            {
                lblMsg.Text = "Enter the 6-digit OTP from your email.";
                return;
            }

            // Verify current password again
            string curHash = Security.Sha256(current);
            object ok = Db.Scalar("SELECT COUNT(1) FROM Users WHERE UserId=@U AND PasswordHash=@H",
                new SqlParameter("@U", userId),
                new SqlParameter("@H", curHash));

            if (Convert.ToInt32(ok) != 1)
            {
                lblMsg.Text = "Current password is incorrect.";
                return;
            }

            if (!OtpService.VerifyAndConsumeOtp(userId, OtpService.PurposeChangePassword, otp, out string err))
            {
                lblMsg.Text = err;
                return;
            }

            string newHash = Security.Sha256(newPw);
            Db.Execute("UPDATE Users SET PasswordHash=@H WHERE UserId=@U",
                new SqlParameter("@H", newHash),
                new SqlParameter("@U", userId));

            lblMsg.Text = "Password changed successfully.";
        }
    }
}
