using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.Util;

namespace Respace
{
    public partial class Rewards : System.Web.UI.Page
    {
        private int UserId => Convert.ToInt32(Session["UserId"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Session check
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // 2. Role check
            var role = Session["Role"]?.ToString() ?? "";
            if (role != "Guest")
            {
                Response.Redirect("Account.aspx");
                return;
            }

            // 3. Initial Load
            if (!IsPostBack)
            {
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            LoadPoints();
            LoadTransactions(); // Points earned
            LoadHistory();      // Vouchers redeemed
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
                ORDER BY CreatedAt DESC", new SqlParameter("@U", UserId));

            gvTransactions.DataSource = dt;
            gvTransactions.DataBind();
        }

        private void LoadHistory()
        {
            // 1. Fetch Vouchers that are NOT USED
            gvAvailableVouchers.DataSource = Db.Query(@"
        SELECT CouponCode, DiscountAmount, IsUsed, RedeemedAt 
        FROM UserCoupons 
        WHERE UserId = @U AND IsUsed = 0
        ORDER BY RedeemedAt DESC", new SqlParameter("@U", UserId));
            gvAvailableVouchers.DataBind();

            // 2. Fetch Vouchers that ARE USED
            gvUsedVouchers.DataSource = Db.Query(@"
        SELECT CouponCode, DiscountAmount, IsUsed, RedeemedAt 
        FROM UserCoupons 
        WHERE UserId = @U AND IsUsed = 1
        ORDER BY RedeemedAt DESC", new SqlParameter("@U", UserId));
            gvUsedVouchers.DataBind();
        }

        protected void RedeemPoints_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] args = btn.CommandArgument.Split('|');
            int pointCost = int.Parse(args[0]);
            decimal discountAmt = decimal.Parse(args[1]);

            // 1. Check current points
            object currentPointsObj = Db.Scalar("SELECT PointsBalance FROM Users WHERE UserId=@U", new SqlParameter("@U", UserId));
            int currentPoints = (currentPointsObj == null) ? 0 : Convert.ToInt32(currentPointsObj);

            if (currentPoints >= pointCost)
            {
                // 2. Subtract points
                Db.Execute("UPDATE Users SET PointsBalance = PointsBalance - @Cost WHERE UserId=@U",
                    new SqlParameter("@Cost", pointCost), new SqlParameter("@U", UserId));

                // 3. Generate a unique code
                string newCode = "REW-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // 4. Insert into UserCoupons
                Db.Execute(@"INSERT INTO UserCoupons (UserId, CouponCode, DiscountAmount, IsUsed, RedeemedAt) 
                             VALUES (@U, @Code, @Amt, 0, GETDATE())",
                    new SqlParameter("@U", UserId),
                    new SqlParameter("@Code", newCode),
                    new SqlParameter("@Amt", discountAmt));

                lblMsg.Text = "Successfully redeemed! Code <strong>" + newCode + "</strong> is now available at checkout.";
                lblMsg.ForeColor = System.Drawing.Color.Green;

                // 5. Refresh the UI to show new balance and history row
                RefreshUI();
            }
            else
            {
                lblMsg.Text = "You do not have enough points.";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}