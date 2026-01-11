using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;

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

            // Only Guest can review (optional rule)
            string role = Session["Role"] == null ? "" : Session["Role"].ToString();
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
                lblMessage.Text = "Invalid space.";
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
            int rating = 0;
            if (!string.IsNullOrEmpty(Request.Form["rating"]))
                int.TryParse(Request.Form["rating"], out rating);

            if (rating < 1 || rating > 5)
            {
                lblMessage.Text = "Please select a star rating.";
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            string comment = (txtComment.Text ?? "").Trim();

            Db.Execute(@"
                INSERT INTO Reviews (SpaceId, UserId, Rating, Comment, IsApproved)
                VALUES (@SpaceId, @UserId, @Rating, @Comment, 0)
            ",
            new SqlParameter("@SpaceId", SpaceId),
            new SqlParameter("@UserId", userId),
            new SqlParameter("@Rating", rating),
            new SqlParameter("@Comment", comment));

            // back to space details or search
            Response.Redirect("SpaceDetails.aspx?id=" + SpaceId);
        }
    }
}
