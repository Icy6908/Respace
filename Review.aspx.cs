using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int RoomId { get; set; }
        public string VenueName { get; set; }
        public int Rating { get; set; }   // 1 - 5
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsApproved { get; set; }
    }

    public partial class review : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadVenue();
        }

        private void LoadVenue()
        {
            int roomId = int.Parse(Request.QueryString["roomId"]);

            var spaces = (List<Space>)Application["Spaces"];
            var venue = spaces.First(s => s.RoomId == roomId);

            txtVenue.Text = venue.Name;
            ViewState["RoomId"] = roomId;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int rating = 0;

            if (!string.IsNullOrEmpty(Request.Form["rating"]))
                rating = int.Parse(Request.Form["rating"]);

            if (rating == 0)
            {
                lblMessage.Text = "Please select a star rating.";
                return;
            }

            var reviews = Application["Reviews"] as List<Review> ?? new List<Review>();

            reviews.Add(new Review
            {
                ReviewId = reviews.Count + 1,
                RoomId = (int)ViewState["RoomId"],
                VenueName = txtVenue.Text,
                Rating = rating,
                Comment = txtComment.Text,
                ReviewDate = DateTime.Now,
                IsApproved = false
            });

            Application["Reviews"] = reviews;
            Response.Redirect("search.aspx");
        }
    }
}

