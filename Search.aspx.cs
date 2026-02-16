using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Respace.App_Code;

namespace Respace
{
    [Serializable]
    public class Space
    {
        public int SpaceId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public decimal PricePerHour { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // ✅ cover photo from SpacePhotos (top 1)
        public string CoverPhotoUrl { get; set; }

        // review summary
        public int AvgRating { get; set; }       // rounded 0-5
        public int ReviewCount { get; set; }     // approved review count
    }

    public partial class Search : Page
    {
        private List<Space> Spaces
        {
            get
            {
                if (ViewState["Spaces"] == null)
                    ViewState["Spaces"] = GetApprovedSpacesFromDb();
                return (List<Space>)ViewState["Spaces"];
            }
            set { ViewState["Spaces"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ✅ allow Search.aspx?q=studio deep link (optional)
                var q = (Request.QueryString["q"] ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(q))
                    txtSearch.Text = q;

                Spaces = GetApprovedSpacesFromDb();
                ApplyFilters(); // binds
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e) => ApplyFilters();

        protected void btnToggleFilter_Click(object sender, EventArgs e)
        {
            pnlFilter.Visible = !pnlFilter.Visible;
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e) => ApplyFilters();

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

            Spaces = GetApprovedSpacesFromDb();
            BindSpaces(Spaces);
        }

        private void ApplyFilters()
        {
            var data = Spaces.AsQueryable();

            // keyword search
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string keyword = txtSearch.Text.Trim().ToLower();
                data = data.Where(s =>
                    (s.Name ?? "").ToLower().Contains(keyword) ||
                    (s.Location ?? "").ToLower().Contains(keyword) ||
                    (s.Type ?? "").ToLower().Contains(keyword) ||
                    (s.Description ?? "").ToLower().Contains(keyword));
            }

            // location filter
            var locations = cblLocation.Items.Cast<ListItem>()
                .Where(i => i.Selected)
                .Select(i => i.Text)
                .ToList();

            if (locations.Any())
                data = data.Where(s => locations.Contains(s.Location));

            // type filter
            var types = cblType.Items.Cast<ListItem>()
                .Where(i => i.Selected)
                .Select(i => i.Text)
                .ToList();

            if (types.Any())
                data = data.Where(s => types.Contains(s.Type));

            // price filter
            if (decimal.TryParse(txtMinPrice.Text, out decimal min))
                data = data.Where(s => s.PricePerHour >= min);

            if (decimal.TryParse(txtMaxPrice.Text, out decimal max))
                data = data.Where(s => s.PricePerHour <= max);

            // availability filter (Bookings + SpaceBlocks)
            if (DateTime.TryParse(txtFromDate.Text, out DateTime fromDate) &&
                DateTime.TryParse(txtToDate.Text, out DateTime toDate))
            {
                DateTime from = fromDate.Date;
                DateTime to = toDate.Date.AddDays(1); // inclusive

                if (to > from)
                {
                    var blockedIds = GetOverlappingUnavailableSpaceIds(from, to);
                    if (blockedIds.Count > 0)
                        data = data.Where(s => !blockedIds.Contains(s.SpaceId));
                }
            }

            // sorting
            switch (ddlSort.SelectedValue)
            {
                case "price_asc":
                    data = data.OrderBy(s => s.PricePerHour);
                    break;

                case "price_desc":
                    data = data.OrderByDescending(s => s.PricePerHour);
                    break;

                case "date_asc":   // label says "Newest: Oldest"
                    data = data.OrderByDescending(s => s.CreatedAt);
                    break;

                case "date_desc":  // label says "Oldest: Newest"
                    data = data.OrderBy(s => s.CreatedAt);
                    break;
            }

            BindSpaces(data.ToList());
        }

        private void BindSpaces(List<Space> data)
        {
            rptSpaces.DataSource = data;
            rptSpaces.DataBind();
        }

        private List<Space> GetApprovedSpacesFromDb()
        {
            // ✅ approved + not deleted + review summary + top 1 cover photo
            DataTable dt = Db.Query(@"
                SELECT
                    s.SpaceId, s.Name, s.Location, s.Type, s.Description,
                    s.PricePerHour, s.Capacity, s.CreatedAt,

                    (SELECT TOP 1 p.PhotoUrl
                     FROM SpacePhotos p
                     WHERE p.SpaceId = s.SpaceId
                     ORDER BY p.IsCover DESC, p.SortOrder ASC, p.PhotoId ASC) AS CoverPhotoUrl,

                    ISNULL(AVG(CASE WHEN r.IsApproved = 1 THEN CAST(r.Rating AS float) END), 0) AS AvgRating,
                    SUM(CASE WHEN r.IsApproved = 1 THEN 1 ELSE 0 END) AS ReviewCount
                FROM Spaces s
                LEFT JOIN Reviews r ON r.SpaceId = s.SpaceId
                WHERE s.Status = 'Approved' AND s.IsDeleted = 0
                GROUP BY s.SpaceId, s.Name, s.Location, s.Type, s.Description,
                         s.PricePerHour, s.Capacity, s.CreatedAt
                ORDER BY s.CreatedAt DESC
            ");

            var list = new List<Space>();

            foreach (DataRow r in dt.Rows)
            {
                double avg = Convert.ToDouble(r["AvgRating"]);
                int avgRounded = (int)Math.Round(avg, MidpointRounding.AwayFromZero);
                if (avgRounded < 0) avgRounded = 0;
                if (avgRounded > 5) avgRounded = 5;

                list.Add(new Space
                {
                    SpaceId = Convert.ToInt32(r["SpaceId"]),
                    Name = r["Name"].ToString(),
                    Location = r["Location"].ToString(),
                    Type = r["Type"].ToString(),
                    Description = r["Description"].ToString(),
                    PricePerHour = Convert.ToDecimal(r["PricePerHour"]),
                    Capacity = Convert.ToInt32(r["Capacity"]),
                    CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                    CoverPhotoUrl = (r["CoverPhotoUrl"] == DBNull.Value) ? "" : r["CoverPhotoUrl"].ToString(),
                    AvgRating = avgRounded,
                    ReviewCount = Convert.ToInt32(r["ReviewCount"])
                });
            }

            return list;
        }

        // ✅ includes Bookings (Confirmed + Pending) AND SpaceBlocks (IsActive=1)
        private HashSet<int> GetOverlappingUnavailableSpaceIds(DateTime from, DateTime to)
        {
            DataTable dt = Db.Query(@"
                SELECT DISTINCT SpaceId
                FROM Bookings
                WHERE Status IN ('Confirmed','Pending')
                  AND NOT (EndDateTime <= @From OR StartDateTime >= @To)

                UNION

                SELECT DISTINCT SpaceId
                FROM SpaceBlocks
                WHERE IsActive = 1
                  AND NOT (EndDate <= @From OR StartDate >= @To)
            ",
            new SqlParameter("@From", from.Date),
            new SqlParameter("@To", to.Date));

            var ids = new HashSet<int>();
            foreach (DataRow row in dt.Rows)
                ids.Add(Convert.ToInt32(row["SpaceId"]));

            return ids;
        }

        // ✅ used by Search.aspx to render stars
        protected string GetStars(int rating)
        {
            if (rating < 0) rating = 0;
            if (rating > 5) rating = 5;
            return new string('★', rating) + new string('☆', 5 - rating);
        }

        // ✅ used by Search.aspx to render cover image nicely (supports http, /uploads, uploads/)
        protected string GetListingImage(object urlObj)
        {
            string url = (urlObj == null) ? "" : urlObj.ToString().Trim();

            if (string.IsNullOrWhiteSpace(url))
            {
                // placeholder (you can replace with your own /Images/placeholder.jpg)
                return "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=1200&q=60";
            }

            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return url;

            // Stored as "uploads/.."
            if (!url.StartsWith("~") && !url.StartsWith("/"))
                url = "~/" + url;

            // Stored as "/uploads/.."
            if (url.StartsWith("/"))
                url = "~" + url;

            return ResolveUrl(url);
        }
    }
}
