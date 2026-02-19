using System;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void btnReset_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            if (Session["ResetUserId"] == null)
            {
                lblMsg.Text = "Session expired. Please request a new OTP.";
                return;
            }

            int userId = Convert.ToInt32(Session["ResetUserId"]);
            string otp = (txtOtp.Text ?? "").Trim();
            string newPw = txtNewPw.Text ?? "";
            string confirm = txtConfirm.Text ?? "";

            if (otp.Length != 6)
            {
                lblMsg.Text = "Please enter a valid 6-digit OTP.";
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

            if (!OtpService.VerifyAndConsumeOtp(userId, OtpService.PurposeForgotPassword, otp, out string err))
            {
                lblMsg.Text = err;
                return;
            }

            string hash = Security.Sha256(newPw);

            Db.Execute("UPDATE Users SET PasswordHash=@H WHERE UserId=@U",
                new SqlParameter("@H", hash),
                new SqlParameter("@U", userId));

            Session.Remove("ResetUserId");

            lblMsg.Text = "Password updated. You can now log in.";
        }
    }
}
