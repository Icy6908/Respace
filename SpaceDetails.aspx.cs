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
            if (SpaceId <= 0)
            {
                lblMsg.Text = "Invalid space.";
                return;
            }

            if (!IsPostBack)
            {
                LoadSpace();
                LoadBookedSlots();
                SetupReviewLink();
                LoadApprovedReviews();
            }
        }

        private void LoadSpace()
        {
            DataTable dt = Db.Query(@"
                SELECT SpaceId, Name, Location, Type, Capacity, PricePerHour, Description
                FROM Spaces
                WHERE SpaceId=@Id AND Status='Approved'
            ", new SqlParameter("@Id", SpaceId));

            if (dt.Rows.Count == 0)
            {
                lblMsg.Text = "Space not found or not approved.";
                pnlDetails.Visible = false;
                return;
            }

            var r = dt.Rows[0];

            pnlDetails.Visible = true;
            lblName.Text = r["Name"].ToString();
            lblLocation.Text = r["Location"].ToString();
            lblType.Text = r["Type"].ToString();
            lblCap.Text = r["Capacity"].ToString();
            lblPrice.Text = Convert.ToDecimal(r["PricePerHour"]).ToString("0.00");
            lblDesc.Text = r["Description"].ToString();
        }

        private void SetupReviewLink()
        {
            // Review page expects ?id=SpaceId
            lnkReview.HRef = "Review.aspx?id=" + SpaceId;
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            // must be logged in
            if (Session["UserId"] == null)
            {
                lblMsg.Text = "Please login to book.";
                return;
            }

            int guestUserId = Convert.ToInt32(Session["UserId"]);

            if (!DateTime.TryParse(txtStartDate.Text, out DateTime sd) ||
                !DateTime.TryParse(txtEndDate.Text, out DateTime ed))
            {
                lblMsg.Text = "Please pick start/end dates.";
                return;
            }

            // time is optional in some browsers; treat empty as 00:00
            TimeSpan st = TimeSpan.Zero, et = TimeSpan.Zero;
            if (!TimeSpan.TryParse(txtStartTime.Text, out st)) st = TimeSpan.Zero;
            if (!TimeSpan.TryParse(txtEndTime.Text, out et)) et = TimeSpan.Zero;

            DateTime start = sd.Date.Add(st);
            DateTime end = ed.Date.Add(et);

            if (end <= start)
            {
                lblMsg.Text = "End datetime must be after start datetime.";
                return;
            }

            // get price/hour
            DataTable dtPrice = Db.Query("SELECT PricePerHour FROM Spaces WHERE SpaceId=@Id",
                new SqlParameter("@Id", SpaceId));
            if (dtPrice.Rows.Count == 0)
            {
                lblMsg.Text = "Space not found.";
                return;
            }

            decimal pricePerHour = Convert.ToDecimal(dtPrice.Rows[0]["PricePerHour"]);
            double hours = (end - start).TotalHours;
            if (hours < 1) hours = 1; // minimum charge 1 hour (optional)

            decimal total = pricePerHour * (decimal)hours;

            // check overlap (confirmed bookings)
            DataTable overlap = Db.Query(@"
                SELECT COUNT(*) AS Cnt
                FROM Bookings
                WHERE SpaceId=@SpaceId
                  AND Status='Confirmed'
                  AND NOT (EndDateTime <= @Start OR StartDateTime >= @End)
            ",
            new SqlParameter("@SpaceId", SpaceId),
            new SqlParameter("@Start", start),
            new SqlParameter("@End", end));

            int cnt = Convert.ToInt32(overlap.Rows[0]["Cnt"]);
            if (cnt > 0)
            {
                lblMsg.Text = "Booking overlaps an existing confirmed booking.";
                return;
            }

            // ✅ IMPORTANT: your schema uses GuestUserId
            Db.Execute(@"
                INSERT INTO Bookings (SpaceId, GuestUserId, StartDateTime, EndDateTime, TotalPrice, Status)
                VALUES (@SpaceId, @GuestUserId, @Start, @End, @Total, 'Confirmed')
            ",
            new SqlParameter("@SpaceId", SpaceId),
            new SqlParameter("@GuestUserId", guestUserId),
            new SqlParameter("@Start", start),
            new SqlParameter("@End", end),
            new SqlParameter("@Total", total));

            Response.Redirect("BookingSuccess.aspx");
        }

        private void LoadApprovedReviews()
        {
            DataTable dt = Db.Query(@"
                SELECT u.FullName AS GuestName, r.Rating, r.Comment, r.CreatedAt
                FROM Reviews r
                INNER JOIN Users u ON u.UserId = r.UserId
                WHERE r.SpaceId=@Id AND r.IsApproved=1
                ORDER BY r.CreatedAt DESC
            ", new SqlParameter("@Id", SpaceId));

            // Add a Stars column string (★★★★★)
            dt.Columns.Add("Stars", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                int rating = Convert.ToInt32(row["Rating"]);
                if (rating < 1) rating = 1;
                if (rating > 5) rating = 5;
                row["Stars"] = new string('★', rating);
            }

            rptReviews.DataSource = dt;
            rptReviews.DataBind();
        }

        private void LoadBookedSlots()
        {
            DataTable dt = Db.Query(@"
        SELECT StartDateTime, EndDateTime
        FROM Bookings
        WHERE SpaceId=@SpaceId AND Status='Confirmed'
        ORDER BY StartDateTime ASC
        ", new SqlParameter("@SpaceId", SpaceId));

            rptBooked.DataSource = dt;
            rptBooked.DataBind();
        }

    }
}
