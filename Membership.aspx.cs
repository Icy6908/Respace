using System;

namespace Respace
{
    public partial class Membership : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Only Guest can access
            var role = Session["Role"]?.ToString() ?? "";
            if (role != "Guest")
            {
                Response.Redirect("Account.aspx");
                return;
            }
        }

        protected void btnFree_Click(object sender, EventArgs e)
        {
            // Fix: Changed GetUserId() to the Session logic you used below
            int userId = Convert.ToInt32(Session["UserId"]);

            MembershipService.ActivatePlan(userId, 1, false);

            // Redirect to Account or refresh to show success
            Response.Redirect("Account.aspx?msg=Plan updated to Free");
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            // Redirect to payment page, passing the PlanId in the URL
            Response.Redirect("Payment.aspx?planId=2");
        }

        protected void btnPro_Click(object sender, EventArgs e)
        {
            // Redirect to payment page, passing the PlanId in the URL
            Response.Redirect("Payment.aspx?planId=3");
        }

        // Note: This helper method isn't being called by your buttons currently 
        // because btnPlus and btnPro redirect to Payment.aspx instead.
        // You will use this logic inside Payment.aspx.cs instead!
        private void ActivatePlan(int planId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            MembershipService.ActivatePlan(userId, planId, true);
            Response.Redirect("Account.aspx");
        }
    }
}