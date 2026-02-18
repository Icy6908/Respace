using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
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

        private int CapacityCached
        {
            get { return (ViewState["Cap"] == null) ? 1 : Convert.ToInt32(ViewState["Cap"]); }
            set { ViewState["Cap"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SpaceId <= 0)
            {
                lblMsg.Text = "<div class='alert error'>Invalid space.</div>";
                pnlDetails.Visible = false;
                return;
            }

            // Always load main record so page stays consistent on postbacks
            if (!LoadSpace())
                return;

            // Always keep calendars + sections up to date
            LoadUnavailableDatesForCalendar();
            LoadPhotos();
            LoadAmenities();

            if (!IsPostBack)
            {
                SetupReviewLink();
                LoadApprovedReviews();
                BindGuestDropdown(CapacityCached);
                RenderMap();
            }
            else
            {
                // Keep map visible on postback too
                RenderMap();
            }
        }

        private bool LoadSpace()
        {
            DataTable dt = Db.Query(@"
                SELECT SpaceId, Name, Location, Type, Capacity, PricePerHour, Description,
                       AddressLine, City, Postcode, State, Country, Latitude, Longitude
                FROM Spaces
                WHERE SpaceId=@Id AND Status='Approved' AND IsDeleted=0
            ", new SqlParameter("@Id", SpaceId));

            if (dt.Rows.Count == 0)
            {
                lblMsg.Text = "<div class='alert error'>Space not found / not approved / deleted.</div>";
                pnlDetails.Visible = false;
                return false;
            }

            var r = dt.Rows[0];
            pnlDetails.Visible = true;

            lblName.Text = r["Name"].ToString();
            lblLocation.Text = r["Location"].ToString();
            lblType.Text = r["Type"].ToString();

            int cap = Convert.ToInt32(r["Capacity"]);
            if (cap < 1) cap = 1;
            CapacityCached = cap;

            lblCap.Text = cap.ToString();
            lblCap2.Text = cap.ToString();
            lblCap3.Text = cap.ToString();

            lblPrice.Text = Convert.ToDecimal(r["PricePerHour"]).ToString("0.00");
            lblPrice2.Text = lblPrice.Text;

            lblDesc.Text = r["Description"].ToString();
            lblType2.Text = lblType.Text;

            string address = JoinNonEmpty(r["AddressLine"], r["City"], r["State"], r["Postcode"], r["Country"]);
            lblAddress.Text = string.IsNullOrWhiteSpace(address) ? "Address not provided." : address;

            ViewState["Lat"] = (r["Latitude"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(r["Latitude"]);
            ViewState["Lng"] = (r["Longitude"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(r["Longitude"]);
            ViewState["AddrText"] = address;

            return true;
        }

        private void BindGuestDropdown(int capacity)
        {
            ddlGuests.Items.Clear();
            for (int i = 1; i <= capacity; i++)
                ddlGuests.Items.Add(i.ToString());
            ddlGuests.SelectedIndex = 0;
        }

        private void LoadPhotos()
        {
            DataTable dt = Db.Query(@"
                SELECT PhotoUrl, IsCover, SortOrder, PhotoId
                FROM SpacePhotos
                WHERE SpaceId=@Id
                ORDER BY IsCover DESC, SortOrder ASC, PhotoId ASC
            ", new SqlParameter("@Id", SpaceId));

            imgCover.Visible = img1.Visible = img2.Visible = img3.Visible = img4.Visible = false;
            pnlNoPhotos.Visible = false;

            var urls = dt.AsEnumerable()
                         .Select(x => (x["PhotoUrl"] ?? "").ToString().Trim())
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .ToList();

            if (urls.Count == 0)
            {
                pnlNoPhotos.Visible = true;
                return;
            }

            SetImg(imgCover, urls.ElementAtOrDefault(0));
            SetImg(img1, urls.ElementAtOrDefault(1));
            SetImg(img2, urls.ElementAtOrDefault(2));
            SetImg(img3, urls.ElementAtOrDefault(3));
            SetImg(img4, urls.ElementAtOrDefault(4));
        }

        // ✅ normalize "uploads/.." and "/uploads/.." into app-relative URL then ResolveUrl
        private void SetImg(System.Web.UI.WebControls.Image img, string url)
        {
            if (img == null) return;

            url = (url ?? "").Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                img.Visible = false;
                return;
            }

            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                img.ImageUrl = url;
                img.Visible = true;
                return;
            }

            // Stored as "uploads/..."
            if (!url.StartsWith("~") && !url.StartsWith("/"))
                url = "~/" + url;

            // Stored as "/uploads/..."
            if (url.StartsWith("/"))
                url = "~" + url;

            img.ImageUrl = ResolveUrl(url);
            img.Visible = true;
        }

        private void LoadAmenities()
        {
            DataTable dt = Db.Query(@"
                SELECT a.AmenityName, a.IconKey
                FROM SpaceAmenities sa
                INNER JOIN Amenities a ON a.AmenityId = sa.AmenityId
                WHERE sa.SpaceId=@Id AND a.IsActive=1
                ORDER BY a.AmenityName
            ", new SqlParameter("@Id", SpaceId));

            if (dt.Rows.Count == 0)
            {
                pnlNoAmenities.Visible = true;
                rptAmenities.DataSource = null;
                rptAmenities.DataBind();
                return;
            }

            pnlNoAmenities.Visible = false;

            if (!dt.Columns.Contains("IconText"))
                dt.Columns.Add("IconText", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                string key = (row["IconKey"] ?? "").ToString().Trim().ToLowerInvariant();
                row["IconText"] = IconKeyToText(key);
            }

            rptAmenities.DataSource = dt;
            rptAmenities.DataBind();
        }

        private string IconKeyToText(string key)
        {
            switch (key)
            {
                case "wifi": return "📶";
                case "projector": return "📽";
                case "whiteboard": return "📝";
                case "ac": return "❄";
                case "sound": return "🔊";
                case "parking": return "🅿";
                case "accessible": return "♿";
                case "kitchen": return "🍽";
                case "tv": return "📺";
                case "mic": return "🎤";
                default: return "✓";
            }
        }

        private void RenderMap()
        {
            decimal? lat = ViewState["Lat"] as decimal?;
            decimal? lng = ViewState["Lng"] as decimal?;
            string addr = (ViewState["AddrText"] ?? "").ToString().Trim();

            if (lat.HasValue && lng.HasValue)
            {
                string q = lat.Value.ToString(CultureInfo.InvariantCulture) + "," +
                           lng.Value.ToString(CultureInfo.InvariantCulture);

                string src = "https://www.google.com/maps?q=" + Uri.EscapeDataString(q) + "&output=embed";
                litMap.Text = $"<iframe class='map-frame' loading='lazy' src='{src}'></iframe>";
                return;
            }

            if (!string.IsNullOrWhiteSpace(addr))
            {
                string src = "https://www.google.com/maps?q=" + Uri.EscapeDataString(addr) + "&output=embed";
                litMap.Text = $"<iframe class='map-frame' loading='lazy' src='{src}'></iframe>";
                return;
            }

            litMap.Text = "<div class='muted small'>Map not available.</div>";
        }

        private void LoadUnavailableDatesForCalendar()
        {
            // bookings
            DataTable dtB = Db.Query(@"
                SELECT StartDateTime, EndDateTime
                FROM Bookings
                WHERE SpaceId=@SpaceId
                  AND Status IN ('Confirmed','Pending')
            ", new SqlParameter("@SpaceId", SpaceId));

            HashSet<string> bookedDays = new HashSet<string>();
            foreach (DataRow row in dtB.Rows)
            {
                DateTime start = Convert.ToDateTime(row["StartDateTime"]).Date;
                DateTime end = Convert.ToDateTime(row["EndDateTime"]).Date;
                for (DateTime d = start; d < end; d = d.AddDays(1))
                    bookedDays.Add(d.ToString("yyyy-MM-dd"));
            }
            hfBookedDates.Value = string.Join(",", bookedDays);

            // blocks
            DataTable dtBlk = Db.Query(@"
                SELECT StartDate, EndDate
                FROM SpaceBlocks
                WHERE SpaceId=@SpaceId AND IsActive=1
            ", new SqlParameter("@SpaceId", SpaceId));

            HashSet<string> blockedDays = new HashSet<string>();
            foreach (DataRow row in dtBlk.Rows)
            {
                DateTime start = Convert.ToDateTime(row["StartDate"]).Date;
                DateTime end = Convert.ToDateTime(row["EndDate"]).Date;
                for (DateTime d = start; d < end; d = d.AddDays(1))
                    blockedDays.Add(d.ToString("yyyy-MM-dd"));
            }
            hfBlockedDates.Value = string.Join(",", blockedDays);
        }

        private void SetupReviewLink()
        {
            lnkReview.HRef = "Review.aspx?id=" + SpaceId;
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

        // Keep your existing btnBook_Click as-is (or paste your booking method here)
        protected void btnBook_Click(object sender, EventArgs e)
        {
            string spaceId = Request.QueryString["id"];
            string start = hfStart.Value;
            string end = hfEnd.Value;
            string guests = ddlGuests.SelectedValue;

            if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
            {
                lblMsg.Text = "Please select a date range.";
                lblMsg.CssClass = "alert error";
                return;
            }

            // Redirect to Checkout with details in QueryString
            Response.Redirect($"Checkout.aspx?id={spaceId}&start={start}&end={end}&guests={guests}");
        }

        private string JoinNonEmpty(params object[] parts)
        {
            var list = new List<string>();
            foreach (var p in parts)
            {
                string s = (p == null || p == DBNull.Value) ? "" : p.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(s))
                    list.Add(s);
            }
            return string.Join(", ", list);
        }

        protected void hfEnd_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
