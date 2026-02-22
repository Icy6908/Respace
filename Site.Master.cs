using System;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                string uid = Session["UserId"].ToString();
                phLoggedIn.Visible = true;
                phLoggedOut.Visible = false;
                phLogout.Visible = true;
                lblUser.Text = $"Hi, {Session["UserName"]} ({Session["Role"]})";

           
                string role = Session["Role"].ToString();
                phHostNav.Visible = (role == "Host");
                phGuestNav.Visible = (role == "Guest");
                phAdmin.Visible = (role == "Admin");

                
                string msgSql = "SELECT COUNT(*) FROM Messages WHERE ReceiverID = @uid AND IsRead = 0";
                int msgCount = Convert.ToInt32(Db.Scalar(msgSql, new SqlParameter("@uid", uid)));
                if (msgCount > 0) { lblUnreadCount.Text = msgCount.ToString(); lblUnreadCount.Visible = true; }

         
                string supportSql = "SELECT COUNT(*) FROM SupportQueries WHERE UserId = @uid AND AdminReply IS NOT NULL AND IsRead = 0";
                int supportCount = Convert.ToInt32(Db.Scalar(supportSql, new SqlParameter("@uid", uid)));
                if (supportCount > 0) { lblSupportBadge.Text = supportCount.ToString(); lblSupportBadge.Visible = true; }
            }
        }
        
        private void UpdateUnreadCount(string uid)
        {
            try
            {
                
                string sql = "SELECT COUNT(*) FROM Messages WHERE ReceiverID = @uid AND IsRead = 0";
                object result = Db.Scalar(sql, new SqlParameter("@uid", uid));

                int count = result != null ? Convert.ToInt32(result) : 0;

                if (count > 0)
                {
                  
                    lblUnreadCount.Text = count > 9 ? "9+" : count.ToString();
                    lblUnreadCount.Visible = true;
                }
                else
                {
                    lblUnreadCount.Visible = false;
                }
            }
            catch
            {
                lblUnreadCount.Visible = false;
            }
        }
    }
}