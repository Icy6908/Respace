using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Respace
{
    public partial class Rewards : System.Web.UI.Page
    {
        private int UserId => Convert.ToInt32(Session["UserId"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            // must be logged in
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // ✅ block Host/Admin: only Guest can use Rewards
            var role = Session["Role"]?.ToString() ?? "";
            if (role != "Guest")
            {
                Response.Redirect("Account.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPoints();
                LoadTransactions();
            }
        }


        private void LoadPoints()
        {
            object balance = Db.Scalar(
                "SELECT PointsBalance FROM Users WHERE UserId=@U",
                new SqlParameter("@U", UserId));

            lblPoints.Text = balance?.ToString() ?? "0";
        }

        private void LoadTransactions()
        {
            DataTable dt = Db.Query(@"
                SELECT TxnType, Points, Reference, CreatedAt
                FROM PointsTransactions
                WHERE UserId=@U
                ORDER BY CreatedAt DESC
            ", new SqlParameter("@U", UserId));

            gvTransactions.DataSource = dt;
            gvTransactions.DataBind();
        }
    }
}
