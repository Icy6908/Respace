using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    [Serializable]
    public class Space
    {
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
                if (ViewState["Spaces"] == null)
                    ViewState["Spaces"] = GetSpaces();

                return (List<Space>)ViewState["Spaces"];
            }
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
                BindSpaces(Spaces);
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
            BindSpaces(Spaces);
        }

        private void ApplyFilters()
        {
            var data = Spaces.AsQueryable();

            // Search keyword
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string keyword = txtSearch.Text.ToLower();
                data = data.Where(s =>
                    s.Name.ToLower().Contains(keyword) ||
                    s.Location.ToLower().Contains(keyword) ||
                    s.Type.ToLower().Contains(keyword));
            }

            // Location filter
            var locations = cblLocation.Items.Cast<System.Web.UI.WebControls.ListItem>()
                .Where(i => i.Selected)
                .Select(i => i.Text)
                .ToList();

            if (locations.Any())
                data = data.Where(s => locations.Contains(s.Location));

            // Type filter
            var types = cblType.Items.Cast<System.Web.UI.WebControls.ListItem>()
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

        private List<Space> GetSpaces()
        {
            return new List<Space>
            {
                new Space { Name="Central Meeting Room", Location="Central", Type="Meeting Room" },
                new Space { Name="East Event Hall", Location="East", Type="Event Hall" },
                new Space { Name="West Training Room", Location="West", Type="Training Room" },
                new Space { Name="North Conference Room", Location="North", Type="Conference Room" },
                new Space { Name="South Studio Space", Location="South", Type="Studio" }
            };
        }
    }
}
