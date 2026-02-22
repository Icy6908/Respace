using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void btnSendOtp_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            string email = (txtEmail.Text ?? "").Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email))
            {
                lblMsg.Text = "Please enter your email.";
                return;
            }

            string generic = "If an account exists, an OTP has been sent to your email.";

            DataTable dt = Db.Query("SELECT UserId, FullName, Email FROM Users WHERE Email=@E",
                new SqlParameter("@E", email));

            if (dt.Rows.Count == 0)
            {
                lblMsg.Text = generic;
                return;
            }

            int userId = Convert.ToInt32(dt.Rows[0]["UserId"]);
            string fullName = dt.Rows[0]["FullName"].ToString();
            string realEmail = dt.Rows[0]["Email"].ToString();

            if (!OtpService.CanSend(userId, OtpService.PurposeForgotPassword, out int wait))
            {
                lblMsg.Text = $"Please wait {wait}s before requesting another OTP.";
                return;
            }

            DateTime exp;
            string otp = OtpService.CreateAndStoreOtp(userId, OtpService.PurposeForgotPassword, out exp);

            string subject = "ReSpace OTP - Reset Password";
            string body = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5'>
                  <h2 style='margin:0 0 8px'>Reset your password</h2>
                  <p>Hi {fullName},</p>
                  <p>Your OTP is:</p>
                  <div style='font-size:28px;font-weight:900;letter-spacing:4px'>{otp}</div>
                  <p style='color:#666'>This OTP expires at {exp:HH:mm}.</p>
                </div>";

            EmailService.Send(realEmail, subject, body);

 
            Session["ResetUserId"] = userId;

            Response.Redirect("ResetPassword.aspx");
        }
    }
}
