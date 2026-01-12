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

        // ✅ NEW: review info
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
                Spaces = GetApprovedSpacesFromDb();
                BindSpaces(Spaces);
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

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string keyword = txtSearch.Text.Trim().ToLower();
                data = data.Where(s =>
                    (s.Name ?? "").ToLower().Contains(keyword) ||
                    (s.Location ?? "").ToLower().Contains(keyword) ||
                    (s.Type ?? "").ToLower().Contains(keyword) ||
                    (s.Description ?? "").ToLower().Contains(keyword));
            }

            var locations = cblLocation.Items.Cast<ListItem>()
                .Where(i => i.Selected).Select(i => i.Text).ToList();
            if (locations.Any())
                data = data.Where(s => locations.Contains(s.Location));

            var types = cblType.Items.Cast<ListItem>()
                .Where(i => i.Selected).Select(i => i.Text).ToList();
            if (types.Any())
                data = data.Where(s => types.Contains(s.Type));

            if (decimal.TryParse(txtMinPrice.Text, out decimal min))
                data = data.Where(s => s.PricePerHour >= min);

            if (decimal.TryParse(txtMaxPrice.Text, out decimal max))
                data = data.Where(s => s.PricePerHour <= max);

            if (DateTime.TryParse(txtFromDate.Text, out DateTime fromDate) &&
                DateTime.TryParse(txtToDate.Text, out DateTime toDate))
            {
                DateTime from = fromDate.Date;
                DateTime to = toDate.Date.AddDays(1);

                var blockedIds = GetOverlappingBookedSpaceIds(from, to);
                if (blockedIds.Count > 0)
                    data = data.Where(s => !blockedIds.Contains(s.SpaceId));
            }

            switch (ddlSort.SelectedValue)
            {
                case "price_asc":
                    data = data.OrderBy(s => s.PricePerHour);
                    break;
                case "price_desc":
                    data = data.OrderByDescending(s => s.PricePerHour);
                    break;
                case "date_asc":
                    data = data.OrderByDescending(s => s.CreatedAt);
                    break;
                case "date_desc":
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
            // ✅ This query also loads APPROVED reviews summary
            DataTable dt = Db.Query(@"
                SELECT
                    s.SpaceId, s.Name, s.Location, s.Type, s.Description,
                    s.PricePerHour, s.Capacity, s.CreatedAt,

                    ISNULL(AVG(CASE WHEN r.IsApproved = 1 THEN CAST(r.Rating AS float) END), 0) AS AvgRating,
                    SUM(CASE WHEN r.IsApproved = 1 THEN 1 ELSE 0 END) AS ReviewCount
                FROM Spaces s
                LEFT JOIN Reviews r ON r.SpaceId = s.SpaceId
                WHERE s.Status = 'Approved'
                GROUP BY s.SpaceId, s.Name, s.Location, s.Type, s.Description,
                         s.PricePerHour, s.Capacity, s.CreatedAt
                ORDER BY s.CreatedAt DESC
            ");

            var list = new List<Space>();
            foreach (DataRow r in dt.Rows)
            {
                double avg = Convert.ToDouble(r["AvgRating"]);
                int avgRounded = (int)Math.Round(avg, MidpointRounding.AwayFromZero);

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
                    AvgRating = avgRounded,
                    ReviewCount = Convert.ToInt32(r["ReviewCount"])
                });
            }

            return list;
        }

        private HashSet<int> GetOverlappingBookedSpaceIds(DateTime from, DateTime to)
        {
            DataTable dt = Db.Query(@"
                SELECT DISTINCT SpaceId
                FROM Bookings
                WHERE Status = 'Confirmed'
                  AND NOT (EndDateTime <= @From OR StartDateTime >= @To)
            ",
            new SqlParameter("@From", from),
            new SqlParameter("@To", to));

            var ids = new HashSet<int>();
            foreach (DataRow r in dt.Rows)
                ids.Add(Convert.ToInt32(r["SpaceId"]));

            return ids;
        }

        // ✅ used by Search.aspx to render stars
        protected string GetStars(int rating)
        {
            if (rating < 0) rating = 0;
            if (rating > 5) rating = 5;
            return new string('★', rating);
        }
    }
}
