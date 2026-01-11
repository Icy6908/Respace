using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;

namespace Respace
{
    public partial class SpaceDetails : System.Web.UI.Page
    {
        private int SpaceId
        {
            get
            {
                int id;
                int.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ make browser show calendar + time picker
            txtStartDate.Attributes["type"] = "date";
            txtEndDate.Attributes["type"] = "date";
            txtStartTime.Attributes["type"] = "time";
            txtEndTime.Attributes["type"] = "time";

            if (!IsPostBack)
            {
                LoadSpace();

                // (optional) set some default values to make testing easier
                txtStartDate.Text = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
                txtEndDate.Text = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
                txtStartTime.Text = "10:00";
                txtEndTime.Text = "12:00";
            }
        }

        private void LoadSpace()
        {
            if (SpaceId <= 0)
            {
                lblMsg.Text = "Invalid space.";
                return;
            }

            DataTable dt = Db.Query(@"
                SELECT SpaceId, Name, Location, Type, Description, PricePerHour, Capacity
                FROM Spaces
                WHERE SpaceId=@Id AND Status='Approved'
            ", new SqlParameter("@Id", SpaceId));

            if (dt.Rows.Count == 0)
            {
                lblMsg.Text = "Space not found or not approved.";
                return;
            }

            pnlDetails.Visible = true;

            lblName.Text = dt.Rows[0]["Name"].ToString();
            lblLocation.Text = dt.Rows[0]["Location"].ToString();
            lblType.Text = dt.Rows[0]["Type"].ToString();
            lblDesc.Text = dt.Rows[0]["Description"].ToString();
            lblCap.Text = dt.Rows[0]["Capacity"].ToString();

            decimal pricePerHour = Convert.ToDecimal(dt.Rows[0]["PricePerHour"]);
            lblPrice.Text = pricePerHour.ToString("0.00");
            ViewState["PricePerHour"] = pricePerHour;
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string role = Session["Role"] == null ? "" : Session["Role"].ToString();
            if (role != "Guest")
            {
                lblMsg.Text = "Only Guest accounts can book.";
                return;
            }

            // ✅ combine date + time into DateTime
            DateTime start, end;

            string startText = (txtStartDate.Text + " " + txtStartTime.Text).Trim();
            string endText = (txtEndDate.Text + " " + txtEndTime.Text).Trim();

            if (!DateTime.TryParse(startText, out start) || !DateTime.TryParse(endText, out end))
            {
                lblMsg.Text = "Please select valid start/end date and time.";
                return;
            }

            if (start < DateTime.Now)
            {
                lblMsg.Text = "Start cannot be in the past.";
                return;
            }

            if (end <= start)
            {
                lblMsg.Text = "End must be after start.";
                return;
            }

            // total price: charged by hour (rounded up)
            decimal pricePerHour = (decimal)ViewState["PricePerHour"];
            double totalHours = (end - start).TotalHours;
            int billHours = (int)Math.Ceiling(totalHours);
            decimal totalPrice = pricePerHour * billHours;

            int guestId = Convert.ToInt32(Session["UserId"]);

            Db.Execute(@"
                INSERT INTO Bookings (SpaceId, GuestUserId, StartDateTime, EndDateTime, TotalPrice, Status)
                VALUES (@SpaceId, @GuestUserId, @Start, @End, @TotalPrice, 'Confirmed')
            ",
            new SqlParameter("@SpaceId", SpaceId),
            new SqlParameter("@GuestUserId", guestId),
            new SqlParameter("@Start", start),
            new SqlParameter("@End", end),
            new SqlParameter("@TotalPrice", totalPrice));

            Response.Redirect("BookingSuccess.aspx");
        }
    }
}
