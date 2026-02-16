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

            phAdmin.Visible = loggedIn && role == "Admin";
            phHostNav.Visible = loggedIn && role == "Host";
            phGuestNav.Visible = loggedIn && role == "Guest";

            lblUser.Text = loggedIn ? ("Hi, " + name + " (" + role + ")") : "";
        }
    }
}
