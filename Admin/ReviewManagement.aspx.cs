using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Respace.App_Code;

namespace Respace.Admin
{
    public partial class ReviewManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { BindReviewGrid(); }
        }

        private void BindReviewGrid()
        {
            // Joining Reviews with Spaces and Users to get both Listing and Profile details
            string query = @"
                SELECT r.*, s.Name as SpaceName, 
                       u.FullName as ReviewerName, u.Email, u.Role, u.CreatedAt as UserJoinedDate
                FROM Reviews r
                JOIN Spaces s ON r.SpaceId = s.SpaceId
                JOIN Users u ON r.UserId = u.UserId
                WHERE r.IsApproved = 0 OR r.IsApproved IS NULL
                ORDER BY r.CreatedAt DESC";

            gvReviews.DataSource = Db.Query(query);
            gvReviews.DataBind();
        }

        protected void gvReviews_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string reviewId = e.CommandArgument.ToString();

            if (e.CommandName == "ApproveReview")
            {
                Db.Query("UPDATE Reviews SET IsApproved = 1 WHERE ReviewId = @id", new SqlParameter("@id", reviewId));
            }
            else if (e.CommandName == "DeleteReview")
            {
                Db.Query("DELETE FROM Reviews WHERE ReviewId = @id", new SqlParameter("@id", reviewId));
            }
            BindReviewGrid();
        }

        public string GenerateStars(int rating)
        {
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                stars += i <= rating ? "★" : "☆";
            }
            return stars;
        }
    }
}