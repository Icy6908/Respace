using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Respace
{
    public partial class ConfirmSpace : System.Web.UI.Page
    {
        private int UserId => (Session["UserId"] == null) ? 0 : Convert.ToInt32(Session["UserId"]);
        private string Role => (Session["Role"] ?? "").ToString();

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
            if (UserId <= 0) { Response.Redirect("Login.aspx"); return; }
            if (!Role.Equals("Host", StringComparison.OrdinalIgnoreCase)) { Response.Redirect("Account.aspx"); return; }
            if (SpaceId <= 0) { ShowError("Invalid listing."); return; }

            if (!IsPostBack)
            {
                LoadAll();
            }
        }

        private void LoadAll()
        {
            
            DataTable dt = Db.Query(@"
                SELECT SpaceId, HostUserId, Name, Location, Type, Capacity, PricePerHour, Description,
                       AddressLine, City, State, Postcode, Country, Latitude, Longitude, Status, IsDeleted
                FROM Spaces
                WHERE SpaceId=@Id AND HostUserId=@U AND IsDeleted=0
            ", new SqlParameter("@Id", SpaceId),
               new SqlParameter("@U", UserId));

            if (dt.Rows.Count == 0) { ShowError("Listing not found / not yours / deleted."); return; }

            var r = dt.Rows[0];

            lblName.Text = r["Name"].ToString();
            lblLocation.Text = r["Location"].ToString();
            lblType.Text = r["Type"].ToString();
            lblCap.Text = r["Capacity"].ToString();
            lblPrice.Text = Convert.ToDecimal(r["PricePerHour"]).ToString("0.00");
            lblDesc.Text = r["Description"].ToString();

            string addr = JoinNonEmpty(r["AddressLine"], r["City"], r["State"], r["Postcode"], r["Country"]);
            lblAddress.Text = string.IsNullOrWhiteSpace(addr) ? "Address not provided." : addr;

            decimal? lat = (r["Latitude"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(r["Latitude"]);
            decimal? lng = (r["Longitude"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(r["Longitude"]);
            RenderMap(lat, lng, addr);

           
            lnkBackToEdit.HRef = "HostCreateSpace.aspx?id=" + SpaceId;

            LoadPhotos();
            LoadAmenities();
        }

        private void LoadPhotos()
        {
            DataTable dt = Db.Query(@"
                SELECT PhotoUrl, IsCover, SortOrder, PhotoId
                FROM SpacePhotos
                WHERE SpaceId=@Id
                ORDER BY IsCover DESC, SortOrder ASC, PhotoId ASC
            ", new SqlParameter("@Id", SpaceId));

            var urls = dt.AsEnumerable()
                         .Select(x => (x["PhotoUrl"] ?? "").ToString().Trim())
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .ToList();

            if (urls.Count == 0)
            {
                pnlNoPhotos.Visible = true;
                rptPhotos.DataSource = null;
                rptPhotos.DataBind();
                return;
            }

            pnlNoPhotos.Visible = false;

         
            var list = urls.Take(5).Select(u => new
            {
                ResolvedUrl = ResolveImg(u)
            }).ToList();

            rptPhotos.DataSource = list;
            rptPhotos.DataBind();
        }

        private string ResolveImg(string url)
        {
            url = (url ?? "").Trim();
            if (string.IsNullOrWhiteSpace(url)) return "";

            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return url;

            if (!url.StartsWith("~") && !url.StartsWith("/"))
                url = "~/" + url;

      
            if (url.StartsWith("/"))
                url = "~" + url;

            return ResolveUrl(url);
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

        private void RenderMap(decimal? lat, decimal? lng, string addr)
        {
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

            litMap.Text = "<div class='muted'>Map not available.</div>";
        }

        protected void btnConfirmSubmit_Click(object sender, EventArgs e)
        {
     
            int affected = Db.Execute(@"
                UPDATE Spaces
                SET Status='Pending'
                WHERE SpaceId=@Id AND HostUserId=@U AND IsDeleted=0
            ", new SqlParameter("@Id", SpaceId),
               new SqlParameter("@U", UserId));

            if (affected <= 0)
            {
                ShowError("Submit failed. Listing not found / not yours.");
                return;
            }

            Response.Redirect("Account.aspx?msg=submitted");
        }

        private void ShowError(string msg)
        {
            lblMsg.Text = "<div class='alert error'>" + Server.HtmlEncode(msg) + "</div>";
        }

        private string JoinNonEmpty(params object[] parts)
        {
            var list = new List<string>();
            foreach (var p in parts)
            {
                string s = (p == null || p == DBNull.Value) ? "" : p.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(s)) list.Add(s);
            }
            return string.Join(", ", list);
        }
    }
}
