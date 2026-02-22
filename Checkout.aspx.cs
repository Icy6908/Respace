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

          
            if (Session["Role"] != null && Session["Role"].ToString() == "Host")
            {
              
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCheckoutDetails();
                LoadUserMembershipInfo();
                LoadAvailableCoupons();
            }
        }

        private void LoadUserMembershipInfo()
        {
            int guestId = Convert.ToInt32(Session["UserId"]);
            double multiplier = 1.0;
            string planName = "Free";

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

            hfMultiplier.Value = multiplier.ToString();
            hfPlanName.Value = planName;
            ScriptManager.RegisterStartupScript(this, GetType(), "initPoints", "updatePointsDisplay();", true);
        }

        private void LoadAvailableCoupons()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            DataTable dt = Db.Query("SELECT UserCouponId, CouponCode, DiscountAmount FROM UserCoupons WHERE UserId=@U AND IsUsed=0",
                new SqlParameter("@U", userId));

            ddlUserCoupons.Items.Clear();
            ddlUserCoupons.Items.Add(new ListItem("Select a redeemed voucher", "0"));

            foreach (DataRow row in dt.Rows)
            {
                string text = $"{row["CouponCode"]} (${row["DiscountAmount"]} Off)";
                ddlUserCoupons.Items.Add(new ListItem(text, row["DiscountAmount"].ToString()));
            }

            object points = Db.Scalar("SELECT PointsBalance FROM Users WHERE UserId=@U", new SqlParameter("@U", userId));
            if (Convert.ToInt32(points) >= 500 && dt.Rows.Count == 0)
            {
                lblPointsReminder.Visible = true;
            }
        }

        protected void btnApplyCoupon_Click(object sender, EventArgs e)
        {
            string inputCode = txtCoupon.Text.Trim().ToUpper();
            decimal discount = 0;

            if (inputCode == "SAVE5")
            {
                discount = 5.00m;
            }
            else
            {
                object dbDiscount = Db.Scalar("SELECT DiscountAmount FROM UserCoupons WHERE CouponCode = @Code AND UserId = @UId AND IsUsed = 0",
                    new SqlParameter("@Code", inputCode),
                    new SqlParameter("@UId", Session["UserId"]));

                if (dbDiscount != null) discount = Convert.ToDecimal(dbDiscount);
            }

            if (discount > 0)
            {
                phCouponDiscount.Visible = true;
                lblCouponAmt.Text = discount.ToString("0.00");
                decimal subtotal = decimal.Parse(lblSubtotal.Text);
                decimal memberDiscount = phMemberDiscount.Visible ? decimal.Parse(lblMemberDiscount.Text) : 0;
                lblTotal.Text = (subtotal - memberDiscount - discount).ToString("0.00");
                lblCouponMsg.Text = "Coupon applied!";
                lblCouponMsg.ForeColor = System.Drawing.Color.Green;
                btnApplyCoupon.Enabled = false;
                ViewState["AppliedCouponCode"] = inputCode;
                ScriptManager.RegisterStartupScript(this, GetType(), "updatePoints", "updatePointsDisplay();", true);
            }
            else
            {
                lblCouponMsg.Text = "Invalid or used coupon code.";
                lblCouponMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void ddlUserCoupons_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal subtotal = decimal.Parse(lblSubtotal.Text);
            decimal memberDiscount = phMemberDiscount.Visible ? decimal.Parse(lblMemberDiscount.Text) : 0;
            decimal rewardDiscount = 0;

            if (ddlUserCoupons.SelectedValue != "0")
            {
                rewardDiscount = decimal.Parse(ddlUserCoupons.SelectedValue);
                phCouponDiscount.Visible = true;
                lblCouponAmt.Text = rewardDiscount.ToString("0.00");
                ViewState["AppliedCouponCode"] = ddlUserCoupons.SelectedItem.Text.Split(' ')[0];
            }
            else
            {
                phCouponDiscount.Visible = false;
                lblCouponAmt.Text = "0.00";
                ViewState["AppliedCouponCode"] = null;
            }

            decimal finalTotal = subtotal - memberDiscount - rewardDiscount;
            lblTotal.Text = (finalTotal > 0 ? finalTotal : 0).ToString("0.00");
            ScriptManager.RegisterStartupScript(this, GetType(), "updatePoints", "updatePointsDisplay();", true);
        }

        protected void btnFinalize_Click(object sender, EventArgs e)
        {
            
            if (Session["Role"] != null && Session["Role"].ToString() == "Host") return;

            int userId = Convert.ToInt32(Session["UserId"]);
            string spaceId = Request.QueryString["id"] ?? Request.QueryString["SpaceId"];
            decimal finalTotal = decimal.Parse(lblTotal.Text);

            try
            {
                object bookingIdObj = Db.Scalar(@"
                    INSERT INTO Bookings (GuestUserId, SpaceId, TotalPrice, Status, CreatedAt, StartDateTime, EndDateTime) 
                    VALUES (@U, @S, @Amt, 'Unpaid', GETDATE(), @Start, @End);
                    SELECT SCOPE_IDENTITY();",
                    new SqlParameter("@U", userId),
                    new SqlParameter("@S", spaceId),
                    new SqlParameter("@Amt", finalTotal),
                    new SqlParameter("@Start", Request.QueryString["start"]),
                    new SqlParameter("@End", Request.QueryString["end"]));

                if (bookingIdObj != null)
                {
                    if (ViewState["AppliedCouponCode"] != null)
                    {
                        string appliedCode = ViewState["AppliedCouponCode"].ToString();
                        Db.Query("UPDATE UserCoupons SET IsUsed = 1 WHERE CouponCode = @Code AND UserId = @UId",
                            new SqlParameter("@Code", appliedCode),
                            new SqlParameter("@UId", userId));
                    }

                    InitializeGuestHostChat(bookingIdObj.ToString(), userId.ToString(), spaceId);
                    Response.Redirect($"Payment.aspx?bid={bookingIdObj}&amt={finalTotal}");
                }
            }
            catch (Exception)
            {
                lblMsg.Text = "There was an error processing your booking.";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void InitializeGuestHostChat(string bookingId, string guestId, string spaceId)
        {
            DataTable dtSpace = Db.Query("SELECT HostUserId FROM Spaces WHERE SpaceId = @sid", new SqlParameter("@sid", spaceId));
            if (dtSpace.Rows.Count > 0)
            {
                string hostId = dtSpace.Rows[0]["HostUserId"].ToString();
                string welcomeMsg = "System: Booking initiated. You can now coordinate your stay here!";
                string msgSql = @"INSERT INTO Messages (SenderID, ReceiverID, BookingID, MessageText, Timestamp, IsRead) 
                                  VALUES (@sender, @receiver, @bid, @txt, GETDATE(), 0)";

                Db.Query(msgSql, new SqlParameter("@sender", guestId), new SqlParameter("@receiver", hostId),
                                 new SqlParameter("@bid", bookingId), new SqlParameter("@txt", welcomeMsg));
            }
        }

        private void LoadCheckoutDetails()
        {
            
            string rawId = Request.QueryString["id"] ?? Request.QueryString["SpaceId"];
            if (string.IsNullOrEmpty(rawId))
            {
                lblMsg.Text = "Invalid Space selection.";
                return;
            }

            int spaceId = Convert.ToInt32(rawId);

           
            DateTime start, end;
            if (!DateTime.TryParse(Request.QueryString["start"], out start)) start = DateTime.Now;
            if (!DateTime.TryParse(Request.QueryString["end"], out end)) end = DateTime.Now.AddDays(1);

            int guests = 1;
            int.TryParse(Request.QueryString["guests"], out guests);

            DataTable dt = Db.Query("SELECT Name, PricePerHour FROM Spaces WHERE SpaceId=@Id", new SqlParameter("@Id", spaceId));
            if (dt.Rows.Count == 0) return;

            decimal dailyRate = Convert.ToDecimal(dt.Rows[0]["PricePerHour"]);
            int nights = (end - start).Days;
            if (nights <= 0) nights = 1;
            decimal subtotal = dailyRate * nights;

            int userId = Convert.ToInt32(Session["UserId"]);

        
            var membership = MembershipService.GetActiveMembership(userId);
            decimal discountPercent = 0;

            if (membership != null)
            {
                if (membership.PlanName == "Plus") discountPercent = 0.10m;
                else if (membership.PlanName == "Pro") discountPercent = 0.20m;
            }

            decimal memberDiscountAmt = subtotal * discountPercent;

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
    }
}