using System;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class HostCreateSpace : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only Host can access
            if (Session["Role"] == null || Session["Role"].ToString() != "Host")
            {
                Response.Redirect("Login.aspx");
                return;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblMsg.ForeColor = System.Drawing.Color.Red;

            // Basic validation
            string name = txtName.Text.Trim();
            string location = ddlLocation.SelectedValue;
            string type = ddlType.SelectedValue;
            string desc = txtDesc.Text.Trim();

            decimal price;
            int cap;

            if (string.IsNullOrWhiteSpace(name))
            {
                lblMsg.Text = "Please enter a space name.";
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Trim(), out price))
            {
                lblMsg.Text = "Price must be a number (e.g. 25.00).";
                return;
            }

            if (!int.TryParse(txtCap.Text.Trim(), out cap) || cap <= 0)
            {
                lblMsg.Text = "Capacity must be a positive whole number.";
                return;
            }

            int hostUserId = Convert.ToInt32(Session["UserId"]);

            Db.Execute(@"
                INSERT INTO Spaces(HostUserId, Name, Location, Type, Description, PricePerHour, Capacity, Status)
                VALUES(@HostUserId, @Name, @Location, @Type, @Description, @PricePerHour, @Capacity, 'Pending')
            ",
            new SqlParameter("@HostUserId", hostUserId),
            new SqlParameter("@Name", name),
            new SqlParameter("@Location", location),
            new SqlParameter("@Type", type),
            new SqlParameter("@Description", (object)desc ?? DBNull.Value),
            new SqlParameter("@PricePerHour", price),
            new SqlParameter("@Capacity", cap));

            lblMsg.ForeColor = System.Drawing.Color.Green;
            lblMsg.Text = "Submitted! Your space is now Pending approval.";

            // Optional: clear form
            txtName.Text = "";
            txtDesc.Text = "";
            txtPrice.Text = "";
            txtCap.Text = "";
        }
    }
}
