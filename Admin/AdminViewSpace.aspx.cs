using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Respace.App_Code;

namespace Respace.Admin
{
    public partial class AdminViewSpace : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id)) { LoadData(id); }
            }
        }

        private void LoadData(string id)
        {
            // 1. Fetch Space and Host Details
            DataTable dtSpace = Db.Query(@"
                SELECT s.*, u.FullName 
                FROM Spaces s 
                JOIN Users u ON s.HostUserId = u.UserId 
                WHERE s.SpaceId = @id", new SqlParameter("@id", id));

            if (dtSpace.Rows.Count > 0)
            {
                DataRow row = dtSpace.Rows[0];
                litSpaceName.Text = row["Name"].ToString();
                litDescription.Text = row["Description"].ToString();
                litType.Text = row["Type"].ToString();
                litCategory.Text = row["Category"].ToString();
                litCapacity.Text = row["Capacity"].ToString();
                // Ensure we use PricePerHour for consistency
                litPrice.Text = string.Format("{0:C}", row["PricePerHour"]);
                litLocation.Text = row["Location"].ToString();
                litHostName.Text = row["FullName"].ToString();
                litHostId.Text = row["HostUserId"].ToString();

                // 2. Fetch PENDING Reviews with User Profile insights
                DataTable dtReviews = Db.Query(@"
                    SELECT r.*, u.FullName as ReviewerName, u.Email, u.Role, u.CreatedAt as UserJoinedDate
                    FROM Reviews r
                    JOIN Users u ON r.UserId = u.UserId
                    WHERE r.SpaceId = @id AND (r.IsApproved = 0 OR r.IsApproved IS NULL)
                    ORDER BY r.CreatedAt DESC", new SqlParameter("@id", id));

                litReviewCount.Text = dtReviews.Rows.Count.ToString();
                rptReviews.DataSource = dtReviews;
                rptReviews.DataBind();
            }
        }

        // Logic to Approve or Delete individual reviews from the list
        protected void rptReviews_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string reviewId = e.CommandArgument.ToString();
            string spaceId = Request.QueryString["id"];

            if (e.CommandName == "ApproveReview")
            {
                Db.Query("UPDATE Reviews SET IsApproved = 1 WHERE ReviewId = @id", new SqlParameter("@id", reviewId));
            }
            else if (e.CommandName == "DeleteReview")
            {
                Db.Query("DELETE FROM Reviews WHERE ReviewId = @id", new SqlParameter("@id", reviewId));
            }

            LoadData(spaceId); // Refresh the UI
        }

        public string GenerateStars(int rating)
        {
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                stars += i <= rating ? "<i class='fas fa-star'></i>" : "<i class='far fa-star'></i>";
            }
            return stars;
        }

        // Space Vetting Logic
        protected void btnApprove_Click(object sender, EventArgs e) => UpdateStatus("Approved");
        protected void btnReject_Click(object sender, EventArgs e) => UpdateStatus("Rejected");

        private void UpdateStatus(string status)
        {
            string id = Request.QueryString["id"];
            Db.Query("UPDATE Spaces SET Status = @status WHERE SpaceId = @id",
                new SqlParameter("@status", status), new SqlParameter("@id", id));
            Response.Redirect("SpaceManagement.aspx");
        }
    }
}