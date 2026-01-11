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
    }

    public partial class Search : System.Web.UI.Page
    {
        private List<Space> Spaces
        {
            get
            {
                // Cache in ViewState so we don't hit DB every postback
                if (ViewState["Spaces"] == null)
                    ViewState["Spaces"] = GetApprovedSpacesFromDb();

                return (List<Space>)ViewState["Spaces"];
            }
            set
            {
                ViewState["Spaces"] = value;
            }
        }

        private bool IsFilterVisible
        {
            get
            {
                return ViewState["FilterVisible"] != null && (bool)ViewState["FilterVisible"];
            }
            set
            {
                ViewState["FilterVisible"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlFilter.Visible = IsFilterVisible;

            if (!IsPostBack)
            {
                // Load approved spaces once at first load
                Spaces = GetApprovedSpacesFromDb();
                BindSpaces(Spaces);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        protected void btnToggleFilter_Click(object sender, EventArgs e)
        {
            IsFilterVisible = !IsFilterVisible;
            pnlFilter.Visible = IsFilterVisible;
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            cblLocation.ClearSelection();
            cblType.ClearSelection();

            // reload fresh from DB
            Spaces = GetApprovedSpacesFromDb();
            BindSpaces(Spaces);
        }

        private void ApplyFilters()
        {
            var data = Spaces.AsQueryable();

            // keyword
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string keyword = txtSearch.Text.Trim().ToLower();
                data = data.Where(s =>
                    (s.Name ?? "").ToLower().Contains(keyword) ||
                    (s.Location ?? "").ToLower().Contains(keyword) ||
                    (s.Type ?? "").ToLower().Contains(keyword));
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
                SELECT SpaceId, Name, Location, Type
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
                    Type = r["Type"].ToString()
                });
            }
            return list;
        }
    }
}
