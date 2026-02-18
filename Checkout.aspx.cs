using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Respace
{
    public partial class Checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCheckoutDetails();

                int guestId = Convert.ToInt32(Session["UserId"]);
                double multiplier = 1.0;
                string planName = "Free";

                // Fetch plan name and multiplier
                object planNameObj = Db.Scalar(@"
            SELECT TOP 1 p.PlanName
            FROM UserMemberships um
            INNER JOIN MembershipPlans p ON p.PlanId = um.PlanId
            WHERE um.UserId = @Uid AND um.IsActive = 1
            ORDER BY um.StartDate DESC", new SqlParameter("@Uid", guestId));

                if (planNameObj != null)
                {
                    planName = planNameObj.ToString();
                    if (planName == "Plus") multiplier = 1.5;
                    else if (planName == "Pro") multiplier = 2.0;
                }

                // Send values to the HiddenFields
                hfMultiplier.Value = multiplier.ToString();
                hfPlanName.Value = planName;

                // Initial call to set the reason text
                ScriptManager.RegisterStartupScript(this, GetType(), "initPoints", "updatePointsDisplay();", true);
            }
        }

        protected void btnApplyCoupon_Click(object sender, EventArgs e)
        {
            if (txtCoupon.Text.Trim().ToUpper() == "SAVE5")
            {
                phCouponDiscount.Visible = true;
                lblCouponAmt.Text = "5.00";

                decimal currentTotal = decimal.Parse(lblTotal.Text);
                lblTotal.Text = (currentTotal - 5.00m).ToString("0.00");

                lblCouponMsg.Text = "Coupon applied!";
                lblCouponMsg.ForeColor = System.Drawing.Color.Green;
                btnApplyCoupon.Enabled = false;

                // IMPORTANT: Tell JavaScript to update points after the total changes
                ScriptManager.RegisterStartupScript(this, GetType(), "updatePoints", "updatePointsDisplay();", true);
            }
            else { /* ... invalid logic ... */ }
        }
        private void LoadCheckoutDetails()
        {
            int spaceId = Convert.ToInt32(Request.QueryString["id"]);
            DateTime start = DateTime.Parse(Request.QueryString["start"]);
            DateTime end = DateTime.Parse(Request.QueryString["end"]);
            int guests = Convert.ToInt32(Request.QueryString["guests"]);

            // 1. Fetch Space Info
            DataTable dt = Db.Query("SELECT Name, PricePerHour FROM Spaces WHERE SpaceId=@Id", new SqlParameter("@Id", spaceId));
            if (dt.Rows.Count == 0) return;

            decimal dailyRate = Convert.ToDecimal(dt.Rows[0]["PricePerHour"]);
            int nights = (end - start).Days;
            if (nights <= 0) nights = 1; // Safety for same-day bookings
            decimal subtotal = dailyRate * nights;

            // 2. Fetch Membership Discount
            int userId = Convert.ToInt32(Session["UserId"]);
            // Assuming you have a MembershipService or similar helper
            var membership = MembershipService.GetActiveMembership(userId);
            decimal discountPercent = 0;

            if (membership.PlanName == "Plus") discountPercent = 0.10m; // 10%
            else if (membership.PlanName == "Pro") discountPercent = 0.20m; // 20%

            decimal memberDiscountAmt = subtotal * discountPercent;

            // 3. Bind UI
            lblSpaceName.Text = dt.Rows[0]["Name"].ToString();
            lblDates.Text = $"{start:dd MMM yyyy} to {end:dd MMM yyyy}";
            lblGuests.Text = guests.ToString();
            lblNights.Text = nights.ToString();
            lblSubtotal.Text = subtotal.ToString("0.00");

            if (memberDiscountAmt > 0)
            {
                phMemberDiscount.Visible = true;
                lblTierName.Text = membership.PlanName;
                lblMemberDiscount.Text = memberDiscountAmt.ToString("0.00");
            }

            lblTotal.Text = (subtotal - memberDiscountAmt).ToString("0.00");
        }

        protected void btnFinalize_Click(object sender, EventArgs e)
        {
            int guestId = Convert.ToInt32(Session["UserId"]);
            int spaceId = Convert.ToInt32(Request.QueryString["id"]);
            DateTime start = DateTime.Parse(Request.QueryString["start"]);
            DateTime end = DateTime.Parse(Request.QueryString["end"]);
            decimal total = decimal.Parse(lblTotal.Text, System.Globalization.NumberStyles.Currency);

            // --- STEP 1: Determine Points Multiplier based on Membership ---
            double multiplier = 1.0; // Default for "Free"

            // Check for an active paid membership
            object planNameObj = Db.Scalar(@"
        SELECT TOP 1 p.PlanName
        FROM UserMemberships um
        INNER JOIN MembershipPlans p ON p.PlanId = um.PlanId
        WHERE um.UserId = @Uid AND um.IsActive = 1
        ORDER BY um.StartDate DESC", new SqlParameter("@Uid", guestId));

            if (planNameObj != null)
            {
                string planName = planNameObj.ToString();
                if (planName == "Plus") multiplier = 1.5;
                else if (planName == "Pro") multiplier = 2.0;
            }

            // --- STEP 2: Calculate Points ---
            int pointsEarned = (int)Math.Floor((double)total * multiplier);

            // --- STEP 3: Insert Booking ---
            string bookingSql = @"INSERT INTO Bookings 
                         (SpaceId, GuestUserId, StartDateTime, EndDateTime, TotalPrice, Status, CreatedAt) 
                         VALUES (@Sid, @Guid, @Start, @End, @Total, 'Pending', GETDATE())";

            Db.Execute(bookingSql,
                new SqlParameter("@Sid", spaceId),
                new SqlParameter("@Guid", guestId),
                new SqlParameter("@Start", start),
                new SqlParameter("@End", end),
                new SqlParameter("@Total", total));

            // --- STEP 4: Update User Points Balance ---
            string pointsSql = @"UPDATE Users 
                         SET PointsBalance = ISNULL(PointsBalance, 0) + @Points 
                         WHERE UserId = @Uid";

            Db.Execute(pointsSql,
                new SqlParameter("@Points", pointsEarned),
                new SqlParameter("@Uid", guestId));

            // Redirect to success
            Response.Redirect($"BookingSuccess.aspx?points={pointsEarned}&mult={multiplier}");
        }
    }
}