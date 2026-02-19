using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Respace
{
    public static class EmailService
    {
        public static bool Send(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var host = ConfigurationManager.AppSettings["SmtpHost"];
                var port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                var user = ConfigurationManager.AppSettings["SmtpUser"];
                var pass = ConfigurationManager.AppSettings["SmtpPass"];
                var from = ConfigurationManager.AppSettings["SmtpFrom"] ?? user;
                var enableSsl = (ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true")
                    .Equals("true", StringComparison.OrdinalIgnoreCase);

                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(from);
                    msg.To.Add(toEmail);
                    msg.Subject = subject;
                    msg.Body = htmlBody;
                    msg.IsBodyHtml = true;

                    using (var smtp = new SmtpClient(host, port))
                    {
                        smtp.EnableSsl = enableSsl;
                        smtp.Credentials = new NetworkCredential(user, pass);
                        smtp.Send(msg);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
