using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Respace.App_Code;
using System.Collections.Generic;

namespace Respace.Admin
{
    public partial class SpaceManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSpaceGrid();
            }
        }

        private void BindSpaceGrid()
        {
            string searchTerm = txtSearchSpace.Text.Trim();
            string selectedType = ddlTypeFilter.SelectedValue;
            string selectedStatus = ddlStatusFilter.SelectedValue;

            // Base query joining Spaces and Users
            string query = @"SELECT s.SpaceId, s.Name, s.Type, s.PricePerHour, s.Status, s.HostUserId, u.FullName as HostName 
                             FROM Spaces s 
                             JOIN Users u ON s.HostUserId = u.UserId 
                             WHERE 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            // 1. Search Filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (s.Name LIKE @search OR u.FullName LIKE @search OR s.Type LIKE @search)";
                parameters.Add(new SqlParameter("@search", "%" + searchTerm + "%"));
            }

            // 2. Space Type Filter
            if (selectedType != "All")
            {
                query += " AND s.Type = @type";
                parameters.Add(new SqlParameter("@type", selectedType));
            }

            // 3. Status Filter
            if (selectedStatus != "All")
            {
                query += " AND s.Status = @status";
                parameters.Add(new SqlParameter("@status", selectedStatus));
            }

            query += " ORDER BY s.SpaceId DESC";

            gvSpaces.DataSource = Db.Query(query, parameters.ToArray());
            gvSpaces.DataBind();
            upSpaceGrid.Update();
        }

        protected void txtSearchSpace_TextChanged(object sender, EventArgs e)
        {
            BindSpaceGrid();
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindSpaceGrid();
        }

        protected void gvSpaces_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string spaceId = e.CommandArgument.ToString();

            if (e.CommandName == "Approve")
            {
                Db.Query("UPDATE Spaces SET Status = 'Approved' WHERE SpaceId = @id", new SqlParameter("@id", spaceId));
                BindSpaceGrid();
            }
            else if (e.CommandName == "Reject")
            {
                Db.Query("UPDATE Spaces SET Status = 'Rejected' WHERE SpaceId = @id", new SqlParameter("@id", spaceId));
                BindSpaceGrid();
            }
            else if (e.CommandName == "DeleteSpace")
            {
                Db.Query("DELETE FROM Spaces WHERE SpaceId = @id", new SqlParameter("@id", spaceId));
                BindSpaceGrid();
            }
            // ViewDetails logic remains unchanged...
        }

        public string GetStatusClass(string status)
        {
            switch (status?.Trim())
            {
                case "Approved": return "bg-success";
                case "Pending": return "bg-warning text-dark";
                case "Rejected": return "bg-danger";
                default: return "bg-secondary";
            }
        }
    }
}