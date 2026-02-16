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

            // only Guest can access
            var role = Session["Role"]?.ToString() ?? "";
            if (role != "Guest")
            {
                Response.Redirect("Account.aspx");
                return;
            }
        }

        protected void btnFree_Click(object sender, EventArgs e)
        {
            ActivatePlan(1);
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            ActivatePlan(2);
        }

        protected void btnPro_Click(object sender, EventArgs e)
        {
            ActivatePlan(3);
        }

        private void ActivatePlan(int planId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            MembershipService.ActivatePlan(userId, planId, true);
            Response.Redirect("Account.aspx");
        }
    }
}
