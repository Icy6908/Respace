using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Respace.App_Code; // Ensure this namespace matches your Db class location

namespace Respace
{
    public partial class Membership : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Authentication Check
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // 2. Role Authorization
            var role = Session["Role"]?.ToString() ?? "";
            if (role != "Guest")
            {
                Response.Redirect("Account.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ShowCurrentMembership();
            }
        }

        private void ShowCurrentMembership()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                // Pulling active plan details
                DataTable dt = Db.Query(@"
                    SELECT p.PlanName 
                    FROM UserMemberships um 
                    JOIN MembershipPlans p ON um.PlanId = p.PlanId 
                    WHERE um.UserId = @uid AND um.IsActive = 1",
                    new SqlParameter("@uid", userId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Ensure phCurrentPlan exists in your .aspx with ID="phCurrentPlan"
                    phCurrentPlan.Visible = true;

                    string planName = dt.Rows[0]["PlanName"].ToString();
                    lblCurrentPlanName.Text = planName;

                    // Disable the button for the current active plan
                    UpdatePlanButtons(planName);
                }
            }
            catch (Exception ex)
            {
                // This will show the error on screen if you have a label named lblMsg
                // lblMsg.Text = "Error loading membership: " + ex.Message;
            }
        }

        private void UpdatePlanButtons(string activePlan)
        {
            if (activePlan.Equals("Free", StringComparison.OrdinalIgnoreCase))
            {
                btnFree.Enabled = false;
                btnFree.Text = "Current Plan";
            }
            else if (activePlan.Equals("Plus", StringComparison.OrdinalIgnoreCase))
            {
                btnPlus.Enabled = false;
                btnPlus.Text = "Current Plan";
                btnPlus.CssClass = "btn-plan"; // Remove primary color if disabled
            }
            else if (activePlan.Equals("Pro", StringComparison.OrdinalIgnoreCase))
            {
                btnPro.Enabled = false;
                btnPro.Text = "Current Plan";
            }
        }

        protected void btnFree_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            // Activate Free plan directly (no payment needed)
            MembershipService.ActivatePlan(userId, 1, false);
            Response.Redirect("Account.aspx?msg=PlanUpdated");
        }

        protected void btnPlus_Click(object sender, EventArgs e)
        {
            Response.Redirect("Payment.aspx?planId=2");
        }

        protected void btnPro_Click(object sender, EventArgs e)
        {
            Response.Redirect("Payment.aspx?planId=3");
        }
    }
}