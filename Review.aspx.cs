using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class Review : System.Web.UI.Page
    {
       
        private int BookingId => int.TryParse(Request.QueryString["bid"], out int id) ? id : 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
                LoadBookingAndSpaceDetails();
        }

        private void LoadBookingAndSpaceDetails()
        {
            if (BookingId <= 0)
            {
                lblMessage.Text = "No booking reference found.";
                btnSubmit.Enabled = false;
                return;
            }

          
            DataTable dt = Db.Query(@"
                SELECT s.Name, s.SpaceId 
                FROM Bookings b
                INNER JOIN Spaces s ON b.SpaceId = s.SpaceId
                WHERE b.BookingId = @Bid AND b.GuestUserId = @Uid
            ",
            new SqlParameter("@Bid", BookingId),
            new SqlParameter("@Uid", Session["UserId"]));

            if (dt.Rows.Count == 0)
            {
                lblMessage.Text = "Booking not found or you do not have permission to review it.";
                btnSubmit.Enabled = false;
                return;
            }

            lblSpaceName.Text = dt.Rows[0]["Name"].ToString();
            
            ViewState["SpaceId"] = dt.Rows[0]["SpaceId"];
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            int spaceId = ViewState["SpaceId"] != null ? (int)ViewState["SpaceId"] : 0;

            if (spaceId == 0)
            {
                lblMessage.Text = "Error identifying space.";
                return;
            }

            int rating = 0;
            int.TryParse(Request.Form["rating"], out rating);

            if (rating == 0)
            {
                lblMessage.Text = "Please select a star rating.";
                return;
            }

            string comment = (txtComment.Text ?? "").Trim();
            string badges = string.Join(", ",
                cblBadges.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value)
            );

            Db.Execute(@"
                INSERT INTO dbo.Reviews (SpaceId, UserId, Rating, Comment, Badges, IsApproved, CreatedAt)
                VALUES (@SpaceId, @UserId, @Rating, @Comment, @Badges, 0, GETDATE())
            ",
            new SqlParameter("@SpaceId", spaceId),
            new SqlParameter("@UserId", userId),
            new SqlParameter("@Rating", rating),
            new SqlParameter("@Comment", comment),
            new SqlParameter("@Badges", badges));

          
            Db.Execute("UPDATE Bookings SET Status = 'Reviewed' WHERE BookingId = @Bid",
                new SqlParameter("@Bid", BookingId));

            Response.Redirect("Account.aspx?msg=ReviewSubmitted");
        }
    }
}