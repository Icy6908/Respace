using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
    }

    public partial class Search : System.Web.UI.Page
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

        private bool IsFilterVisible
        {
            get => ViewState["FilterVisible"] != null && (bool)ViewState["FilterVisible"];
            set => ViewState["FilterVisible"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlFilter.Visible = IsFilterVisible;

            if (!IsPostBack)
            {
                Spaces = GetApprovedSpacesFromDb();
                BindSpaces(Spaces);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e) => ApplyFilters();

        protected void btnToggleFilter_Click(object sender, EventArgs e)
        {
            IsFilterVisible = !IsFilterVisible;
            pnlFilter.Visible = IsFilterVisible;
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
                .Where(i => i.Selected).Select(i => i.Text).ToList();
            if (locations.Any())
                data = data.Where(s => locations.Contains(s.Location));

            // type filter
            var types = cblType.Items.Cast<ListItem>()
                .Where(i => i.Selected).Select(i => i.Text).ToList();
            if (types.Any())
                data = data.Where(s => types.Contains(s.Type));

            // price filter
            if (decimal.TryParse(txtMinPrice.Text, out decimal min))
                data = data.Where(s => s.PricePerHour >= min);

            if (decimal.TryParse(txtMaxPrice.Text, out decimal max))
                data = data.Where(s => s.PricePerHour <= max);

            // availability filter (optional) - removes spaces with overlapping confirmed bookings
            if (DateTime.TryParse(txtFromDate.Text, out DateTime fromDate) &&
                DateTime.TryParse(txtToDate.Text, out DateTime toDate))
            {
                // treat as full-day range
                DateTime from = fromDate.Date;
                DateTime to = toDate.Date.AddDays(1); // exclusive end (next day 00:00)

                var blockedIds = GetOverlappingBookedSpaceIds(from, to);
                if (blockedIds.Count > 0)
                    data = data.Where(s => !blockedIds.Contains(s.SpaceId));
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
                case "date_asc":
                    data = data.OrderByDescending(s => s.CreatedAt); // "Newest: Oldest"
                    break;
                case "date_desc":
                    data = data.OrderBy(s => s.CreatedAt); // "Oldest: Newest"
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
            DataTable dt = Db.Query(@"
                SELECT SpaceId, Name, Location, Type, Description, PricePerHour, Capacity, CreatedAt
                FROM Spaces
                WHERE Status = 'Approved'
                ORDER BY CreatedAt DESC
            ");

            var list = new List<Space>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Space
                {
                    SpaceId = Convert.ToInt32(r["SpaceId"]),
                    Name = r["Name"].ToString(),
                    Location = r["Location"].ToString(),
                    Type = r["Type"].ToString(),
                    Description = r["Description"].ToString(),
                    PricePerHour = Convert.ToDecimal(r["PricePerHour"]),
                    Capacity = Convert.ToInt32(r["Capacity"]),
                    CreatedAt = Convert.ToDateTime(r["CreatedAt"])
                });
            }
            return list;
        }

        private HashSet<int> GetOverlappingBookedSpaceIds(DateTime from, DateTime to)
        {
            // overlap rule: NOT (End <= from OR Start >= to)
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
    }
}
