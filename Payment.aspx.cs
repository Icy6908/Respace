using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class Payment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) Response.Redirect("Login.aspx");
            if (!IsPostBack) LoadPurchaseDetails();
        }

        private void LoadPurchaseDetails()
        {
            if (Request.QueryString["planId"] != null)
            {
                phMembershipInfo.Visible = true;
                int planId = Convert.ToInt32(Request.QueryString["planId"]);

                // Use 'MonthlyFee' to match your database schema
                DataTable dt = Db.Query("SELECT PlanName, MonthlyFee FROM MembershipPlans WHERE PlanId = @id",
                                        new SqlParameter("@id", planId));

                if (dt.Rows.Count > 0)
                {
                    lblPlanName.Text = dt.Rows[0]["PlanName"].ToString();
                    lblTotalAmount.Text = Convert.ToDecimal(dt.Rows[0]["MonthlyFee"]).ToString("0.00");
                }
            }
            else if (Request.QueryString["bid"] != null)
            {
                phBookingInfo.Visible = true;
                lblBookingId.Text = Request.QueryString["bid"];
                lblTotalAmount.Text = Request.QueryString["amt"];
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            if (Request.QueryString["planId"] != null)
            {
                int planId = Convert.ToInt32(Request.QueryString["planId"]);
                MembershipService.ActivatePlan(userId, planId, true);
                Response.Redirect("Account.aspx?msg=MembershipActivated");
            }
            else if (Request.QueryString["bid"] != null)
            {
                int bookingId = Convert.ToInt32(Request.QueryString["bid"]);
                decimal amount = decimal.Parse(Request.QueryString["amt"]);

                ProcessBookingPayment(userId, bookingId, amount);
                Response.Redirect("Account.aspx?msg=BookingConfirmed");
            }
        }

        private void ProcessBookingPayment(int userId, int bookingId, decimal amount)
        {
            // Status is set to 'Confirmed' based on your Bookings table defaults
            Db.Execute("UPDATE Bookings SET Status = 'Confirmed' WHERE BookingId = @bid AND GuestUserId = @uid",
                new SqlParameter("@bid", bookingId), new SqlParameter("@uid", userId));

            double multiplier = GetUserMultiplier(userId);
            int points = (int)Math.Floor((double)amount * multiplier);

            Db.Execute("UPDATE Users SET PointsBalance = ISNULL(PointsBalance, 0) + @pts WHERE UserId = @uid",
                new SqlParameter("@pts", points), new SqlParameter("@uid", userId));

            Db.Execute(@"INSERT INTO PointsTransactions (UserId, TxnType, Points, Reference, CreatedAt) 
                         VALUES (@uid, 'Earned', @pts, @ref, GETDATE())",
                new SqlParameter("@uid", userId), new SqlParameter("@pts", points),
                new SqlParameter("@ref", "Booking #" + bookingId));
        }

        // This method is now properly placed inside the class to fix the red underline
        private double GetUserMultiplier(int userId)
        {
            object multiplier = Db.Scalar(@"SELECT TOP 1 p.PointsMultiplier FROM UserMemberships um 
                JOIN MembershipPlans p ON p.PlanId = um.PlanId 
                WHERE um.UserId = @uid AND um.IsActive = 1 ORDER BY um.StartDate DESC",
                new SqlParameter("@uid", userId));

            return multiplier != null ? Convert.ToDouble(multiplier) : 1.0;
        }
    }
}