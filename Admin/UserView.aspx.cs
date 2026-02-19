using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace.Admin
{
    public partial class UserView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string userId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(userId))
                {
                    // Calling methods defined below
                    LoadUserProfile(userId);
                    LoadUserBookings(userId);
                }
            }
        }

        private void LoadUserProfile(string userId)
        {
            // Fetch user info from the Users table
            string query = "SELECT FullName, Email, Role, PointsBalance, MembershipTier, CreatedAt, Status FROM Users WHERE UserId = @id";
            DataTable dt = Db.Query(query, new SqlParameter("@id", userId));

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                litFullName.Text = row["FullName"].ToString();
                litEmail.Text = row["Email"].ToString();
                litRole.Text = row["Role"].ToString();
                litPoints.Text = row["PointsBalance"].ToString();
                litTier.Text = row["MembershipTier"].ToString();
                litJoined.Text = Convert.ToDateTime(row["CreatedAt"]).ToString("MMMM dd, yyyy");
                litStatus.Text = row["Status"].ToString();
            }
        }

        private void LoadUserBookings(string userId)
        {
            // UPDATED: Using StartDateTime, EndDateTime, and TotalPrice
            string query = @"SELECT b.BookingId, s.Name as SpaceName, b.StartDateTime, b.EndDateTime, b.TotalPrice, b.Status 
                     FROM Bookings b 
                     JOIN Spaces s ON b.SpaceId = s.SpaceId 
                     WHERE b.GuestUserId = @uid 
                     ORDER BY b.StartDateTime DESC";

            gvUserBookings.DataSource = Db.Query(query, new SqlParameter("@uid", userId));
            gvUserBookings.DataBind();
        }
    }
}   