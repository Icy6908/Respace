using Respace.App_Code;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

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

            if (!IsPostBack)
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                // Load coupons they OWN but haven't used
                DataTable dt = Db.Query("SELECT UserCouponId, CouponCode, DiscountAmount FROM UserCoupons WHERE UserId=@U AND IsUsed=0",
                    new SqlParameter("@U", userId));

                foreach (DataRow row in dt.Rows)
                {
                    string text = $"{row["CouponCode"]} (${row["DiscountAmount"]} Off)";
                    ddlUserCoupons.Items.Add(new ListItem(text, row["DiscountAmount"].ToString()));
                }

                // Optional: Reminder if they have high points but no coupons
                object points = Db.Scalar("SELECT PointsBalance FROM Users WHERE UserId=@U", new SqlParameter("@U", userId));
                if (Convert.ToInt32(points) >= 500 && dt.Rows.Count == 0)
                {
                    lblPointsReminder.Visible = true;
                }
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
            // 1. Get User and Space details
            int userId = Convert.ToInt32(Session["UserId"]);
            // Using "id" to match your previous logic, or "SpaceId" based on your preference
            string spaceId = Request.QueryString["id"] ?? Request.QueryString["SpaceId"];
            decimal finalTotal = decimal.Parse(lblTotal.Text);

            // 2. Calculate points (using your hidden field multiplier)
            decimal multiplier = 1.0m;
            if (hfMultiplier != null && !string.IsNullOrEmpty(hfMultiplier.Value))
            {
                decimal.TryParse(hfMultiplier.Value, out multiplier);
            }
            int pointsToEarn = (int)Math.Floor(finalTotal * multiplier);

            try
            {
                // 3. Save the Booking with 'Unpaid' status
                // We use SCOPE_IDENTITY to get the new ID for the payment page
                object bookingIdObj = Db.Scalar(@"
            INSERT INTO Bookings (GuestUserId, SpaceId, TotalPrice, Status, CreatedAt, StartDateTime, EndDateTime) 
            VALUES (@U, @S, @Amt, 'Unpaid', GETDATE(), @Start, @End);
            SELECT SCOPE_IDENTITY();",
                    new SqlParameter("@U", userId),
                    new SqlParameter("@S", spaceId),
                    new SqlParameter("@Amt", finalTotal),
                    new SqlParameter("@Start", Request.QueryString["start"]),
                    new SqlParameter("@End", Request.QueryString["end"]));

                // 4. Redirect to Payment Page
                // Matches your Payment.aspx.cs which looks for "bid" and "amt"
                Response.Redirect($"Payment.aspx?bid={bookingIdObj}&amt={finalTotal}");
            }
            catch (Exception)
            {
                // Warning fixed: removed the 'ex' variable name since it wasn't being used
                lblMsg.Text = "There was an error processing your booking. Please try again.";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected void ddlUserCoupons_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. Always start with the original Base Price
            decimal subtotal = decimal.Parse(lblSubtotal.Text);

            // 2. Get the Membership Discount (if visible)
            decimal memberDiscount = 0;
            if (phMemberDiscount.Visible)
            {
                // Use TryParse to prevent crashing if the label is empty
                decimal.TryParse(lblMemberDiscount.Text, out memberDiscount);
            }

            // 3. Get the Reward Coupon Discount from the DropDown
            decimal rewardDiscount = 0;
            if (ddlUserCoupons.SelectedValue != "0")
            {
                rewardDiscount = decimal.Parse(ddlUserCoupons.SelectedValue);

                // Visual feedback: Show the coupon row in the Price Details sidebar
                phCouponDiscount.Visible = true;
                lblCouponAmt.Text = rewardDiscount.ToString("0.00");
            }
            else
            {
                // If they switch back to "Apply coupon", hide the discount row
                phCouponDiscount.Visible = false;
                lblCouponAmt.Text = "0.00";
            }

            // 4. Final Calculation: Total = Base - Membership - Reward
            decimal finalTotal = subtotal - memberDiscount - rewardDiscount;

            // 5. Update UI (Ensure it never goes below zero)
            lblTotal.Text = (finalTotal > 0 ? finalTotal : 0).ToString("0.00");

            // 6. IMPORTANT: Tell JavaScript to update the "Points you'll earn" box
            ScriptManager.RegisterStartupScript(this, GetType(), "updatePoints", "updatePointsDisplay();", true);
        }
    }
}