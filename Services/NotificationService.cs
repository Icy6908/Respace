using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Respace
{
    public static class NotificationService
    {
        public static void SendBookingConfirmed(int bookingId)
        {
            var dt = GetBookingInfo(bookingId);
            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            string space = r["SpaceName"].ToString();
            string location = r["Location"].ToString();
            string guestName = r["GuestName"].ToString();
            string guestEmail = r["GuestEmail"].ToString();
            string hostName = r["HostName"].ToString();
            string hostEmail = r["HostEmail"].ToString();
            DateTime start = Convert.ToDateTime(r["StartDateTime"]);
            DateTime end = Convert.ToDateTime(r["EndDateTime"]);
            decimal total = Convert.ToDecimal(r["TotalPrice"]);

            string subjectGuest = $"ReSpace Booking Confirmed #{bookingId}";
            string bodyGuest = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5'>
                  <h2 style='margin:0 0 8px'>Booking confirmed ✅</h2>
                  <p>Hi {Esc(guestName)},</p>
                  <p>Your booking has been confirmed.</p>
                  <div style='padding:12px;border:1px solid #eee;border-radius:10px'>
                    <b>Space:</b> {Esc(space)}<br/>
                    <b>Location:</b> {Esc(location)}<br/>
                    <b>Start:</b> {start:dd MMM yyyy HH:mm}<br/>
                    <b>End:</b> {end:dd MMM yyyy HH:mm}<br/>
                    <b>Total:</b> {total:0.00}<br/>
                    <b>Booking ID:</b> {bookingId}
                  </div>
                </div>";

            string subjectHost = $"New Booking Confirmed #{bookingId}";
            string bodyHost = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5'>
                  <h2 style='margin:0 0 8px'>New booking confirmed ✅</h2>
                  <p>Hi {Esc(hostName)},</p>
                  <p>A guest booking has been confirmed for your listing.</p>
                  <div style='padding:12px;border:1px solid #eee;border-radius:10px'>
                    <b>Space:</b> {Esc(space)}<br/>
                    <b>Guest:</b> {Esc(guestName)}<br/>
                    <b>Start:</b> {start:dd MMM yyyy HH:mm}<br/>
                    <b>End:</b> {end:dd MMM yyyy HH:mm}<br/>
                    <b>Total:</b> {total:0.00}<br/>
                    <b>Booking ID:</b> {bookingId}
                  </div>
                </div>";

            EmailService.Send(guestEmail, subjectGuest, bodyGuest);
            EmailService.Send(hostEmail, subjectHost, bodyHost);
        }

        public static void SendBookingCancelled(int bookingId, string cancelledBy, string reason = "")
        {
            var dt = GetBookingInfo(bookingId);
            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            string space = r["SpaceName"].ToString();
            string guestName = r["GuestName"].ToString();
            string guestEmail = r["GuestEmail"].ToString();
            string hostName = r["HostName"].ToString();
            string hostEmail = r["HostEmail"].ToString();
            DateTime start = Convert.ToDateTime(r["StartDateTime"]);
            DateTime end = Convert.ToDateTime(r["EndDateTime"]);

            string reasonHtml = string.IsNullOrWhiteSpace(reason) ? "" : $"<br/><b>Reason:</b> {Esc(reason)}";

            string subject = $"ReSpace Booking Cancelled #{bookingId}";

            string bodyGuest = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5'>
                  <h2 style='margin:0 0 8px'>Booking cancelled ❌</h2>
                  <p>Hi {Esc(guestName)},</p>
                  <p>This booking was cancelled by <b>{Esc(cancelledBy)}</b>.</p>
                  <div style='padding:12px;border:1px solid #eee;border-radius:10px'>
                    <b>Space:</b> {Esc(space)}<br/>
                    <b>Start:</b> {start:dd MMM yyyy HH:mm}<br/>
                    <b>End:</b> {end:dd MMM yyyy HH:mm}
                    {reasonHtml}
                  </div>
                </div>";

            string bodyHost = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.5'>
                  <h2 style='margin:0 0 8px'>Booking cancelled ❌</h2>
                  <p>Hi {Esc(hostName)},</p>
                  <p>This booking was cancelled by <b>{Esc(cancelledBy)}</b>.</p>
                  <div style='padding:12px;border:1px solid #eee;border-radius:10px'>
                    <b>Space:</b> {Esc(space)}<br/>
                    <b>Guest:</b> {Esc(guestName)}<br/>
                    <b>Start:</b> {start:dd MMM yyyy HH:mm}<br/>
                    <b>End:</b> {end:dd MMM yyyy HH:mm}
                    {reasonHtml}
                  </div>
                </div>";

            EmailService.Send(guestEmail, subject, bodyGuest);
            EmailService.Send(hostEmail, subject, bodyHost);
        }

        private static DataTable GetBookingInfo(int bookingId)
        {
            return Db.Query(@"
                SELECT 
                    b.BookingId,
                    b.StartDateTime,
                    b.EndDateTime,
                    b.TotalPrice,
                    s.Name AS SpaceName,
                    s.Location,
                    g.FullName AS GuestName,
                    g.Email AS GuestEmail,
                    h.FullName AS HostName,
                    h.Email AS HostEmail
                FROM Bookings b
                INNER JOIN Spaces s ON s.SpaceId = b.SpaceId
                INNER JOIN Users g ON g.UserId = b.GuestUserId
                INNER JOIN Users h ON h.UserId = s.HostUserId
                WHERE b.BookingId = @B",
                new SqlParameter("@B", bookingId));
        }

        private static string Esc(string s)
        {
            return (s ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
