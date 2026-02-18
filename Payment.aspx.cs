using System;

namespace Respace
{
    public partial class Payment : System.Web.UI.Page
    {
        // Ensure this exact name and signature exists
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int planId = Convert.ToInt32(Request.QueryString["planId"]);

                MembershipService.ActivatePlan(userId, planId, true);
                Response.Redirect("Account.aspx?msg=Success");
            }
            catch (Exception ex)
            {
                // Error handling
            }
        }
    }
}