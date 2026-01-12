using System;

namespace Respace
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = Session["UserId"];
            var role = Session["Role"] == null ? "" : Session["Role"].ToString();
            var name = Session["FullName"] == null ? "" : Session["FullName"].ToString();

            bool loggedIn = userId != null;

            phLoggedOut.Visible = !loggedIn;
            phLogout.Visible = loggedIn;
            phLoggedIn.Visible = loggedIn;

            phHost.Visible = loggedIn && role == "Host";
            phAdmin.Visible = loggedIn && role == "Admin";

            lblUser.Text = loggedIn ? ("Hi, " + name + " (" + role + ")") : "";
        }
    }
}
