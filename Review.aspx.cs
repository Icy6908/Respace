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
        private int SpaceId
        {
            get
            {
                int id;
                int.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string role = (Session["Role"] ?? "").ToString();
            if (role != "Guest")
            {
                lblMessage.Text = "Only Guest accounts can submit reviews.";
                btnSubmit.Enabled = false;
                return;
            }

            if (!IsPostBack)
                LoadSpaceName();
        }

        private void LoadSpaceName()
        {
            if (SpaceId <= 0)
            {
                lblMessage.Text = "Invalid space. Please go back and click Write Review again.";
                btnSubmit.Enabled = false;
                return;
            }

            DataTable dt = Db.Query(@"
                SELECT Name
                FROM Spaces
                WHERE SpaceId=@Id AND Status='Approved'
            ", new SqlParameter("@Id", SpaceId));

            if (dt.Rows.Count == 0)
            {
                lblMessage.Text = "Space not found or not approved.";
                btnSubmit.Enabled = false;
                return;
            }

            lblSpaceName.Text = dt.Rows[0]["Name"].ToString();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            int spaceId;
            if (!int.TryParse(Request.QueryString["id"], out spaceId))
            {
                lblMessage.Text = "Invalid space.";
                return;
            }

            // rating is from your HTML radio group named="rating"
            int rating = 0;
            int.TryParse(Request.Form["rating"], out rating);

            string comment = (txtComment.Text ?? "").Trim();

            // ✅ Collect selected compliments
            string badges = string.Join(", ",
                cblBadges.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value)
            );

            // ✅ Insert review WITH badges
            Db.Execute(@"
        INSERT INTO dbo.Reviews (SpaceId, UserId, Rating, Comment, Badges, IsApproved, CreatedAt)
        VALUES (@SpaceId, @UserId, @Rating, @Comment, @Badges, 0, GETDATE())
    ",
            new SqlParameter("@SpaceId", spaceId),
            new SqlParameter("@UserId", userId),
            new SqlParameter("@Rating", rating),
            new SqlParameter("@Comment", comment),
            new SqlParameter("@Badges", badges));

            lblMessage.Text = "Review submitted! Pending approval.";
            Response.Redirect("SpaceDetails.aspx?id=" + spaceId);
        }

    }
}
