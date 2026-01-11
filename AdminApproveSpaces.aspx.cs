using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class AdminApproveSpaces : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only Admin
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPending();
                LoadRecent();
            }
        }

        private void LoadPending()
        {
            DataTable dt = Db.Query(@"
                SELECT S.SpaceId, S.Name, S.Location, S.Type, S.PricePerHour, S.Capacity, S.CreatedAt,
                       U.FullName AS HostName
                FROM Spaces S
                JOIN Users U ON U.UserId = S.HostUserId
                WHERE S.Status = 'Pending'
                ORDER BY S.CreatedAt DESC
            ");

            gvPending.DataSource = dt;
            gvPending.DataBind();
        }

        private void LoadRecent()
        {
            DataTable dt = Db.Query(@"
                SELECT TOP 20 SpaceId, Name, Location, Type, Status, AdminRemarks, CreatedAt
                FROM Spaces
                WHERE Status <> 'Pending'
                ORDER BY CreatedAt DESC
            ");

            gvRecent.DataSource = dt;
            gvRecent.DataBind();
        }

        protected void gvPending_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            lblMsg.ForeColor = System.Drawing.Color.Green;
            lblMsg.Text = "";

            int spaceId;
            if (!int.TryParse(e.CommandArgument.ToString(), out spaceId))
                return;

            // Find the row to read remarks textbox
            GridViewRow row = ((Control)e.CommandSource).NamingContainer as GridViewRow;
            string remarks = "";
            if (row != null)
            {
                TextBox txt = row.FindControl("txtRemarks") as TextBox;
                if (txt != null) remarks = txt.Text.Trim();
            }

            if (e.CommandName == "ApproveSpace")
            {
                Db.Execute(@"
                    UPDATE Spaces
                    SET Status='Approved', AdminRemarks=@Remarks
                    WHERE SpaceId=@Id
                ",
                new SqlParameter("@Remarks", string.IsNullOrWhiteSpace(remarks) ? (object)DBNull.Value : remarks),
                new SqlParameter("@Id", spaceId));

                lblMsg.Text = "Approved Space ID " + spaceId;
            }
            else if (e.CommandName == "RejectSpace")
            {
                Db.Execute(@"
                    UPDATE Spaces
                    SET Status='Rejected', AdminRemarks=@Remarks
                    WHERE SpaceId=@Id
                ",
                new SqlParameter("@Remarks", string.IsNullOrWhiteSpace(remarks) ? (object)DBNull.Value : remarks),
                new SqlParameter("@Id", spaceId));

                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Text = "Rejected Space ID " + spaceId;
            }

            LoadPending();
            LoadRecent();
        }
    }
}
