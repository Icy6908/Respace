using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class adminreview : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Bind(null, null);
        }

        protected void Bind(object sender, EventArgs e)
        {
            var reviews = Application["Reviews"] as List<Review> ?? new List<Review>();

            reviews = reviews.Where(r => r.IsApproved == false).ToList();

            if (!string.IsNullOrEmpty(txtSearch.Text))
                reviews = reviews
                    .Where(r => r.VenueName.ToLower().Contains(txtSearch.Text.ToLower()))
                    .ToList();

            if (ddlSort.SelectedValue == "rating_desc")
                reviews = reviews.OrderByDescending(r => r.Rating).ToList();
            else
                reviews = reviews.OrderByDescending(r => r.ReviewDate).ToList();

            gvReviews.DataSource = reviews;
            gvReviews.DataBind();
        }

        protected void gvReviews_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            var reviews = Application["Reviews"] as List<Review>;

            var pending = reviews.Where(r => r.IsApproved == false).ToList();

            if (e.CommandName == "Approve")
            {
                pending[index].IsApproved = true;
                lblMessage.Text = "Review approved and added to search.aspx";
            }
            else if (e.CommandName == "DeleteReview")
            {
                reviews.Remove(pending[index]);
                lblMessage.Text = "Review deleted";
            }

            Application["Reviews"] = reviews;
            Bind(null, null);
        }
    }
}
