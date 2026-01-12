using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class HostMySpaces : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Host")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
                LoadMySpaces();
        }

        private void LoadMySpaces()
        {
            int hostUserId = Convert.ToInt32(Session["UserId"]);

            DataTable dt = Db.Query(@"
                SELECT SpaceId, Name, Location, Type, PricePerHour, Capacity, Status, CreatedAt
                FROM Spaces
                WHERE HostUserId=@HostUserId
                ORDER BY CreatedAt DESC
            ", new SqlParameter("@HostUserId", hostUserId));

            gv.DataSource = dt;
            gv.DataBind();
        }

        protected void gv_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int hostUserId = Convert.ToInt32(Session["UserId"]);
            int spaceId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ManageBookings")
            {
                Response.Redirect("HostBookings.aspx?spaceId=" + spaceId);
                return;
            }

            if (e.CommandName == "RemoveListing")
            {
                int rows = Db.Execute(@"
                    UPDATE Spaces
                    SET Status='Removed'
                    WHERE SpaceId=@SpaceId AND HostUserId=@HostUserId
                ",
                new SqlParameter("@SpaceId", spaceId),
                new SqlParameter("@HostUserId", hostUserId));

                lblMsg.ForeColor = rows > 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                lblMsg.Text = rows > 0 ? "Listing removed." : "Unable to remove listing.";

                LoadMySpaces();
            }
        }
    }
}
