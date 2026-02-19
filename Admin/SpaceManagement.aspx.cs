using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Respace.App_Code;

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

        private void BindSpaceGrid(string searchTerm = "")
        {
            // Updated to use 'Type' column from your DB schema
            string query = @"SELECT s.SpaceId, s.Name, s.Type, s.PricePerHour, s.Status, s.HostUserId, u.FullName as HostName 
                     FROM Spaces s 
                     JOIN Users u ON s.HostUserId = u.UserId";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " WHERE s.Name LIKE @search OR u.FullName LIKE @search OR s.Type LIKE @search";
                DataTable dt = Db.Query(query, new SqlParameter("@search", "%" + searchTerm + "%"));
                gvSpaces.DataSource = dt;
            }
            else
            {
                DataTable dt = Db.Query(query);
                gvSpaces.DataSource = dt;
            }
            gvSpaces.DataBind();
        }

        protected void txtSearchSpace_TextChanged(object sender, EventArgs e)
        {
            BindSpaceGrid(txtSearchSpace.Text.Trim());
        }

        protected void gvSpaces_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string spaceId = e.CommandArgument.ToString();

            // 1. Logic for "View" - Must be its own IF block
            if (e.CommandName == "ViewDetails")
            {
                DataTable dt = Db.Query(@"
            SELECT s.*, u.FullName, 
            (SELECT COUNT(*) FROM Reviews r WHERE r.SpaceId = s.SpaceId) as ReviewCount
            FROM Spaces s 
            JOIN Users u ON s.HostUserId = u.UserId 
            WHERE s.SpaceId = @id",
                    new SqlParameter("@id", spaceId));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // Setting the values for your Modal Literals
                    litSpaceName.Text = row["Name"].ToString();
                    litDescription.Text = !string.IsNullOrEmpty(row["Description"].ToString()) ? row["Description"].ToString() : "No description provided.";
                    litType.Text = row["Type"].ToString();
                    litCategory.Text = row["Category"].ToString();
                    litCapacity.Text = row["Capacity"].ToString();
                    litPrice.Text = string.Format("{0:C}", row["Price"]);
                    litLocation.Text = row["Location"].ToString(); // Matches your DB 'Location' column
                    litHostName.Text = row["FullName"].ToString();
                    litReviewCount.Text = row["ReviewCount"].ToString();

                    // The 'Magic' line that fixes the refresh issue
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "var myModal = new bootstrap.Modal(document.getElementById('detailsModal')); myModal.show();", true);
                }
            }
            // 2. Separate logic for Approve/Reject so they don't conflict
            else if (e.CommandName == "Approve")
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
        }

        public string GetStatusClass(string status)
        {
            switch (status)
            {
                case "Approved": return "bg-success";
                case "Pending": return "bg-warning text-dark";
                case "Rejected": return "bg-danger";
                default: return "bg-secondary";
            }
        }
    }
}