using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Respace.App_Code;
using System.Text;

namespace Respace.Admin
{
    public partial class ReviewManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { BindReviewGrid(); }
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindReviewGrid();
        }

        private void BindReviewGrid()
        {
            string selectedStar = ddlStarFilter.SelectedValue;
            string searchText = txtSearchReview.Text.Trim();

           
            StringBuilder query = new StringBuilder(@"
                SELECT r.*, s.Name as SpaceName, 
                       u.FullName as ReviewerName, u.Email, u.Role, u.CreatedAt as UserJoinedDate
                FROM Reviews r
                JOIN Spaces s ON r.SpaceId = s.SpaceId
                JOIN Users u ON r.UserId = u.UserId
                WHERE (r.IsApproved = 0 OR r.IsApproved IS NULL)");

            List<SqlParameter> parameters = new List<SqlParameter>();

           
            if (selectedStar != "All")
            {
                query.Append(" AND r.Rating = @rating");
                parameters.Add(new SqlParameter("@rating", selectedStar));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query.Append(" AND (s.Name LIKE @search OR u.FullName LIKE @search OR u.Email LIKE @search)");
                parameters.Add(new SqlParameter("@search", "%" + searchText + "%"));
            }

            query.Append(" ORDER BY r.CreatedAt DESC");

            gvReviews.DataSource = Db.Query(query.ToString(), parameters.ToArray());
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

        public string DisplayBadges(object badgesObj)
        {
            if (badgesObj == null || badgesObj == DBNull.Value || string.IsNullOrEmpty(badgesObj.ToString()))
                return "";

            string[] badges = badgesObj.ToString().Split(',');
            StringBuilder sb = new StringBuilder();
            foreach (string b in badges)
            {
                if (!string.IsNullOrWhiteSpace(b))
                {
                    sb.Append($"<span class='badge-pill-respace'>{b.Trim()}</span>");
                }
            }
            return sb.ToString();
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