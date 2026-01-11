using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Respace
{
    [Serializable]
    public class Space
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime AvailableDate { get; set; }

    }

    public partial class Search : System.Web.UI.Page
    {
        private List<Space> Spaces
        {
            get
            {
                if (ViewState["Spaces"] == null)
                    ViewState["Spaces"] = SeedSpaces();
                return (List<Space>)ViewState["Spaces"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Application["Spaces"] == null)
                {
                    Application["Spaces"] = SeedSpaces();
                }

                Bind(Spaces);
            }
        }
        

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        protected void btnToggleFilter_Click(object sender, EventArgs e)
        {
            pnlFilter.Visible = !pnlFilter.Visible;
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtFromDate.Text = "";
            txtToDate.Text = "";
            txtMinPrice.Text = "";
            txtMaxPrice.Text = "";
            ddlSort.SelectedIndex = 0;
            cblLocation.ClearSelection();
            cblType.ClearSelection();

            Bind(Spaces);
        }

        private void ApplyFilters()
        {
            var data = Spaces.AsQueryable();

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string keyword = txtSearch.Text.ToLower();
                data = data.Where(s =>
                    s.Name.ToLower().Contains(keyword) ||
                    s.Location.ToLower().Contains(keyword) ||
                    s.Type.ToLower().Contains(keyword) ||
                    s.AvailableDate.ToString("MMMM").ToLower().Contains(keyword) ||
                    s.AvailableDate.ToString("MMM").ToLower().Contains(keyword)
                );
            }

       
            var locations = cblLocation.Items.Cast<System.Web.UI.WebControls.ListItem>()
                .Where(i => i.Selected).Select(i => i.Text).ToList();

            if (locations.Any())
                data = data.Where(s => locations.Contains(s.Location));

    
            var types = cblType.Items.Cast<System.Web.UI.WebControls.ListItem>()
                .Where(i => i.Selected).Select(i => i.Text).ToList();

            if (types.Any())
                data = data.Where(s => types.Contains(s.Type));

       
            if (decimal.TryParse(txtMinPrice.Text, out decimal min))
                data = data.Where(s => s.Price >= min);

            if (decimal.TryParse(txtMaxPrice.Text, out decimal max))
                data = data.Where(s => s.Price <= max);

            if (DateTime.TryParse(txtFromDate.Text, out DateTime from))
                data = data.Where(s => s.AvailableDate >= from);

            if (DateTime.TryParse(txtToDate.Text, out DateTime to))
                data = data.Where(s => s.AvailableDate <= to);

            switch (ddlSort.SelectedValue)
            {
                case "price_asc":
                    data = data.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    data = data.OrderByDescending(s => s.Price);
                    break;
                case "date_asc":
                    data = data.OrderBy(s => s.AvailableDate);
                    break;
                case "date_desc":
                    data = data.OrderByDescending(s => s.AvailableDate);
                    break;
            }

            Bind(data.ToList());
        }

        private void Bind(List<Space> data)
        {
            rptSpaces.DataSource = data;
            rptSpaces.DataBind();
        }

        private List<Space> SeedSpaces()
        {
            return new List<Space>
    {
        new Space { RoomId=1, Name="Central Loft", Location="Central", Type="Meeting Room", Price=120, Description="Modern meeting space in CBD", AvailableDate=new DateTime(2026,3,15)},
        new Space { RoomId=2, Name="East Creative Studio", Location="East", Type="Studio", Price=80, Description="Bright creative studio", AvailableDate=new DateTime(2026,4,10)},
        new Space { RoomId=3, Name="West Event Hall", Location="West", Type="Event Hall", Price=300, Description="Large hall for events", AvailableDate=new DateTime(2026,3,25)},
        new Space { RoomId=4, Name="North Training Hub", Location="North", Type="Training Room", Price=150, Description="Corporate training venue", AvailableDate=new DateTime(2026,5,5)},
        new Space { RoomId=5, Name="South Conference Suite", Location="South", Type="Conference Room", Price=200, Description="Premium conference room", AvailableDate=new DateTime(2026,6,1)},

        new Space { RoomId=6, Name="Central Boardroom", Location="Central", Type="Conference Room", Price=220, Description="Executive boardroom", AvailableDate=new DateTime(2026,2,20)},
        new Space { RoomId=7, Name="East Workshop Space", Location="East", Type="Training Room", Price=130, Description="Hands-on workshop venue", AvailableDate=new DateTime(2026,7,12)},
        new Space { RoomId=8, Name="West Open Studio", Location="West", Type="Studio", Price=90, Description="Open layout studio", AvailableDate=new DateTime(2026,8,18)},
        new Space { RoomId=9, Name="North Meeting Pod", Location="North", Type="Meeting Room", Price=70, Description="Small team meeting room", AvailableDate=new DateTime(2026,1,30)},
        new Space { RoomId=10, Name="South Event Pavilion", Location="South", Type="Event Hall", Price=280, Description="Spacious event pavilion", AvailableDate=new DateTime(2026,9,5)},

        new Space { RoomId=11, Name="Central Innovation Lab", Location="Central", Type="Studio", Price=160, Description="Innovation and brainstorming space", AvailableDate=new DateTime(2026,10,10)},
        new Space { RoomId=12, Name="East Conference Plus", Location="East", Type="Conference Room", Price=190, Description="Mid-sized conference room", AvailableDate=new DateTime(2026,11,15)},
        new Space { RoomId=13, Name="West Training Centre", Location="West", Type="Training Room", Price=140, Description="Training centre with AV support", AvailableDate=new DateTime(2026,12,1)},
        new Space { RoomId=14, Name="North Executive Suite", Location="North", Type="Conference Room", Price=250, Description="Executive-level meeting suite", AvailableDate=new DateTime(2026,3,8)},
        new Space { RoomId=15, Name="South Creative Loft", Location="South", Type="Studio", Price=110, Description="Loft-style creative venue", AvailableDate=new DateTime(2026,4,22)},

        new Space { RoomId=16, Name="Central Seminar Room", Location="Central", Type="Training Room", Price=170, Description="Seminar-ready training room", AvailableDate=new DateTime(2026,5,18)},
        new Space { RoomId=17, Name="East Meeting Hub", Location="East", Type="Meeting Room", Price=95, Description="Casual meeting hub", AvailableDate=new DateTime(2026,6,25)},
        new Space { RoomId=18, Name="West Conference Hall", Location="West", Type="Conference Room", Price=210, Description="Conference hall with stage", AvailableDate=new DateTime(2026,7,30)},
        new Space { RoomId=19, Name="North Event Arena", Location="North", Type="Event Hall", Price=350, Description="Large-scale event arena", AvailableDate=new DateTime(2026,8,12)},
        new Space { RoomId=20, Name="South Meeting Lounge", Location="South", Type="Meeting Room", Price=85, Description="Relaxed meeting lounge", AvailableDate=new DateTime(2026,9,20)}
    };
        }
        public class Review
        {
            public int ReviewId { get; set; }
            public int RoomId { get; set; }
            public string VenueName { get; set; }
            public int Rating { get; set; }   // 1 - 5
            public string Comment { get; set; }
            public DateTime ReviewDate { get; set; }
            public bool IsApproved { get; internal set; }
        }
        protected int GetAverageRating(int roomId)
        {
            var reviews = Application["Reviews"] as List<Review>;
            if (reviews == null) return 0;

            var approved = reviews
                .Where(r => r.RoomId == roomId && r.IsApproved)
                .ToList();

            if (!approved.Any()) return 0;

            return (int)Math.Round(approved.Average(r => r.Rating));
        }

        protected List<Review> GetApprovedReviews(int roomId)
        {
            var reviews = Application["Reviews"] as List<Review>;
            if (reviews == null) return new List<Review>();

            return reviews
                .Where(r => r.RoomId == roomId && r.IsApproved)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
        }






    }


}