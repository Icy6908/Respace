using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class Rewards : System.Web.UI.Page
    {
      
        private int UserId => Convert.ToInt32(Session["UserId"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

      
            var role = Session["Role"]?.ToString() ?? "";
            if (role != "Guest")
            {
                Response.Redirect("Account.aspx");
                return;
            }

            
            if (!IsPostBack)
            {
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            LoadPoints();
            LoadVoucherShop();
            LoadTransactions(); 
            LoadHistory();     
        }

        private void LoadPoints()
        {
            
            object balance = Db.Scalar(
                "SELECT PointsBalance FROM Users WHERE UserId=@U",
                new SqlParameter("@U", UserId));

            lblPoints.Text = balance?.ToString() ?? "0";
        }

        private void LoadVoucherShop()
        {
            string sql = "SELECT * FROM CouponDefinitions WHERE IsActive = 1 ORDER BY PointCost ASC";
            DataTable dt = Db.Query(sql);

            rptVoucherShop.DataSource = dt;
            rptVoucherShop.DataBind();
        }

        private void LoadTransactions()
        {
            
            DataTable dt = Db.Query(@"
                SELECT TxnType, Points, Reference, CreatedAt
                FROM PointsTransactions
                WHERE UserId=@U
                ORDER BY CreatedAt DESC", new SqlParameter("@U", UserId));

            gvTransactions.DataSource = dt;
            gvTransactions.DataBind();
        }

        private void LoadHistory()
        {
            
            gvAvailableVouchers.DataSource = Db.Query(@"
                SELECT CouponCode, DiscountAmount, RedeemedAt 
                FROM UserCoupons 
                WHERE UserId = @U AND IsUsed = 0
                ORDER BY RedeemedAt DESC", new SqlParameter("@U", UserId));
            gvAvailableVouchers.DataBind();
        }

        protected void RedeemPoints_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] args = btn.CommandArgument.Split('|');

            int pointCost = int.Parse(args[0]);
            decimal discountAmt = decimal.Parse(args[1]);

            
            object currentPointsObj = Db.Scalar("SELECT PointsBalance FROM Users WHERE UserId=@U",
                new SqlParameter("@U", UserId));
            int currentPoints = (currentPointsObj == null) ? 0 : Convert.ToInt32(currentPointsObj);

            if (currentPoints >= pointCost)
            {
                Db.Execute("UPDATE Users SET PointsBalance = PointsBalance - @Cost WHERE UserId=@U",
                    new SqlParameter("@Cost", pointCost), new SqlParameter("@U", UserId));

                string newCode = "REW-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

             
                Db.Execute(@"INSERT INTO UserCoupons (UserId, CouponCode, DiscountAmount, IsUsed, RedeemedAt) 
                             VALUES (@U, @Code, @Amt, 0, GETDATE())",
                    new SqlParameter("@U", UserId),
                    new SqlParameter("@Code", newCode),
                    new SqlParameter("@Amt", discountAmt));

                
                Db.Execute(@"INSERT INTO PointsTransactions (UserId, Points, TxnType, Reference, CreatedAt)
                             VALUES (@U, @P, 'Redeem', @Ref, GETDATE())",
                    new SqlParameter("@U", UserId),
                    new SqlParameter("@P", -pointCost),
                    new SqlParameter("@Ref", "Redeemed " + newCode));

                lblMsg.Text = "Successfully redeemed! Code <strong>" + newCode + "</strong> is now available.";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                lblMsg.Visible = true;

             
                RefreshUI();
            }
            else
            {
                lblMsg.Text = "You do not have enough points.";
                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Visible = true;
            }
        }
    }
}