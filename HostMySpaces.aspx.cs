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
    }
}
