using Respace.App_Code;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Respace
{
    public static class OtpService
    {
        public const string PurposeForgotPassword = "ForgotPassword";
        public const string PurposeChangePassword = "ChangePassword";

        private static int ExpiryMinutes =>
            int.TryParse(ConfigurationManager.AppSettings["OtpExpiryMinutes"], out int v) ? v : 10;

        private static int CooldownSeconds =>
            int.TryParse(ConfigurationManager.AppSettings["OtpCooldownSeconds"], out int v) ? v : 60;

        private static string Secret =>
            ConfigurationManager.AppSettings["OtpSecret"] ?? "CHANGE_ME";

        public static bool CanSend(int userId, string purpose, out int waitSeconds)
        {
            waitSeconds = 0;

            object lastObj = Db.Scalar(@"
                SELECT TOP 1 CreatedAt
                FROM PasswordOtps
                WHERE UserId=@U AND Purpose=@P
                ORDER BY CreatedAt DESC",
                new SqlParameter("@U", userId),
                new SqlParameter("@P", purpose));

            if (lastObj == null || lastObj == DBNull.Value) return true;

            DateTime last = Convert.ToDateTime(lastObj);
            var elapsed = (DateTime.Now - last).TotalSeconds;

            if (elapsed >= CooldownSeconds) return true;

            waitSeconds = (int)Math.Ceiling(CooldownSeconds - elapsed);
            return false;
        }

        public static string CreateAndStoreOtp(int userId, string purpose, out DateTime expiresAt)
        {
            var rnd = new Random();
            string otp = rnd.Next(100000, 999999).ToString();

            expiresAt = DateTime.Now.AddMinutes(ExpiryMinutes);
            string hash = HashOtp(userId, purpose, otp);

            Db.Execute(@"
                INSERT INTO PasswordOtps (UserId, Purpose, OtpHash, ExpiresAt)
                VALUES (@U, @P, @H, @E)",
                new SqlParameter("@U", userId),
                new SqlParameter("@P", purpose),
                new SqlParameter("@H", hash),
                new SqlParameter("@E", expiresAt));

            return otp;
        }

        public static bool VerifyAndConsumeOtp(int userId, string purpose, string otp, out string error)
        {
            error = "";

            DataTable dt = Db.Query(@"
                SELECT TOP 1 OtpId, OtpHash, ExpiresAt, Attempts
                FROM PasswordOtps
                WHERE UserId=@U AND Purpose=@P AND UsedAt IS NULL
                ORDER BY CreatedAt DESC",
                new SqlParameter("@U", userId),
                new SqlParameter("@P", purpose));

            if (dt.Rows.Count == 0)
            {
                error = "OTP not found. Please request a new one.";
                return false;
            }

            var row = dt.Rows[0];
            int otpId = Convert.ToInt32(row["OtpId"]);
            DateTime exp = Convert.ToDateTime(row["ExpiresAt"]);
            int attempts = Convert.ToInt32(row["Attempts"]);

            if (DateTime.Now > exp)
            {
                error = "OTP expired. Please request a new one.";
                return false;
            }

            if (attempts >= 8)
            {
                error = "Too many attempts. Please request a new OTP.";
                return false;
            }

            string expected = row["OtpHash"].ToString();
            string actual = HashOtp(userId, purpose, otp);

            Db.Execute("UPDATE PasswordOtps SET Attempts = Attempts + 1 WHERE OtpId=@Id",
                new SqlParameter("@Id", otpId));

            if (!string.Equals(expected, actual, StringComparison.Ordinal))
            {
                error = "Invalid OTP.";
                return false;
            }

            Db.Execute("UPDATE PasswordOtps SET UsedAt = GETDATE() WHERE OtpId=@Id",
                new SqlParameter("@Id", otpId));

            return true;
        }

        private static string HashOtp(int userId, string purpose, string otp)
        {
            return Security.Sha256($"{userId}|{purpose}|{otp}|{Secret}");
        }
    }
}
