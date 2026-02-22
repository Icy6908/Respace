using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class AdminReview : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || (Session["Role"]?.ToString() != "Admin"))
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
                Bind(null, null);
        }

        protected void Bind(object sender, EventArgs e)
        {
            string keyword = (txtSearch.Text ?? "").Trim().ToLower();

            string orderBy = "r.CreatedAt DESC";
            if (ddlSort.SelectedValue == "rating_desc")
                orderBy = "r.Rating DESC, r.CreatedAt DESC";

            string sql = @"
    SELECT
        r.ReviewId,
        s.Name AS SpaceName,
        u.FullName AS GuestName,
        r.Rating,
        ISNULL(r.Badges,'') AS Badges,
        r.Comment,
        r.CreatedAt
    FROM dbo.Reviews r
    JOIN dbo.Spaces s ON s.SpaceId = r.SpaceId
    JOIN dbo.Users u ON u.UserId = r.UserId
    WHERE r.IsApproved = 0
      AND (@K = '' OR LOWER(s.Name) LIKE '%' + @K + '%')
    ORDER BY " + orderBy;

            DataTable dt = Db.Query(sql,
                new SqlParameter("@K", keyword)
            );

            gvReviews.DataSource = dt;
            gvReviews.DataBind();
        }


        protected void gvReviews_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            if (rowIndex < 0 || rowIndex >= gvReviews.Rows.Count) return;

            int reviewId = Convert.ToInt32(gvReviews.DataKeys[rowIndex].Value);

            if (e.CommandName == "Approve")
            {
                Db.Execute("UPDATE Reviews SET IsApproved=1 WHERE ReviewId=@Id",
                    new SqlParameter("@Id", reviewId));
                lblMessage.Text = "Review approved.";
            }
            else if (e.CommandName == "DeleteReview")
            {
                Db.Execute("DELETE FROM Reviews WHERE ReviewId=@Id",
                    new SqlParameter("@Id", reviewId));
                lblMessage.Text = "Review deleted.";
            }

            Bind(null, null);
        }
        protected string[] GetBadgesWithEmoji(string badges)
        {
            if (string.IsNullOrWhiteSpace(badges))
                return new string[0];

           
            var parts = badges.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(x => x.Trim())
                              .ToList();

            for (int i = 0; i < parts.Count; i++)
            {
                string b = parts[i];

                switch (b)
                {
                    case "Clean & Comfy": parts[i] = "🧼 " + b; break;
                    case "Friendly Owner": parts[i] = "😊 " + b; break;
                    case "Fairly Priced": parts[i] = "💸 " + b; break;
                    case "Great Amenities": parts[i] = "✨ " + b; break;
                    case "Good Location": parts[i] = "📍 " + b; break;
                    case "Quiet Space": parts[i] = "🤫 " + b; break;
                    case "Easy Check-in": parts[i] = "✅ " + b; break;
                    case "Fast Response": parts[i] = "⚡ " + b; break;
                    default: parts[i] = "🏷️ " + b; break;
                }
            }

            return parts.ToArray();
        }

    }
}
