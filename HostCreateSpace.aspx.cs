using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Respace
{
    public partial class HostCreateSpace : Page
    {
        private int UserId => (Session["UserId"] == null) ? 0 : Convert.ToInt32(Session["UserId"]);
        private string Role => (Session["Role"] ?? "").ToString();

        private int EditingSpaceId
        {
            get
            {
                int id;
                int.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        private int CurrentSpaceId
        {
            get { return (ViewState["SpaceId"] == null) ? 0 : Convert.ToInt32(ViewState["SpaceId"]); }
            set { ViewState["SpaceId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserId <= 0)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!string.Equals(Role, "Host", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("Account.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindAmenities();

                if (EditingSpaceId > 0)
                {
                    if (!HostOwnsSpace(EditingSpaceId))
                    {
                        lblMsg.Text = "<div class='alert error'>Not found / not your listing.</div>";
                        return;
                    }

                    lblTitle.Text = "Edit listing";
                    LoadSpaceForEdit(EditingSpaceId);
                    CurrentSpaceId = EditingSpaceId;

                    LoadPhotosGrid(CurrentSpaceId);
                }
                else
                {
                    // ✅ Option A: create as Draft first, submit later via confirmation page
                    lblTitle.Text = "Create listing";
                    lblStatus.Text = "Draft";
                    lblStatusHint.Text = "(Draft is not visible in Search until submitted)";

                    txtCapacity.Text = "1";
                    txtPrice.Text = "0";
                }
            }
        }

        private bool HostOwnsSpace(int spaceId)
        {
            object ok = Db.Scalar(@"
                SELECT COUNT(*)
                FROM Spaces
                WHERE SpaceId=@S AND HostUserId=@U AND IsDeleted=0
            ",
            new SqlParameter("@S", spaceId),
            new SqlParameter("@U", UserId));

            return Convert.ToInt32(ok) > 0;
        }

        private void BindAmenities()
        {
            DataTable dt = Db.Query(@"
                SELECT AmenityId, AmenityName
                FROM Amenities
                WHERE IsActive=1
                ORDER BY AmenityName
            ");

            cblAmenities.DataSource = dt;
            cblAmenities.DataTextField = "AmenityName";
            cblAmenities.DataValueField = "AmenityId";
            cblAmenities.DataBind();
        }

        private void LoadSpaceForEdit(int spaceId)
        {
            DataTable dt = Db.Query(@"
                SELECT SpaceId, Name, Location, Type, Description, PricePerHour, Capacity, Status,
                       AddressLine, City, Postcode, State, Country, Latitude, Longitude
                FROM Spaces
                WHERE SpaceId=@S AND HostUserId=@U AND IsDeleted=0
            ",
            new SqlParameter("@S", spaceId),
            new SqlParameter("@U", UserId));

            if (dt.Rows.Count == 0)
            {
                lblMsg.Text = "<div class='alert error'>Listing not found.</div>";
                return;
            }

            var r = dt.Rows[0];

            txtName.Text = r["Name"].ToString();
            ddlLocation.SelectedValue = r["Location"].ToString();
            ddlType.SelectedValue = r["Type"].ToString();
            txtDesc.Text = r["Description"].ToString();

            txtPrice.Text = Convert.ToDecimal(r["PricePerHour"]).ToString("0.##", CultureInfo.InvariantCulture);
            txtCapacity.Text = Convert.ToInt32(r["Capacity"]).ToString();

            string status = (r["Status"] ?? "Draft").ToString().Trim();
            if (string.IsNullOrWhiteSpace(status)) status = "Draft";

            lblStatus.Text = status;

            if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                lblStatusHint.Text = "(Approved: visible in Search)";
            else if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                lblStatusHint.Text = "(Pending: admin will approve before it shows in Search)";
            else
                lblStatusHint.Text = "(Draft: not visible in Search until submitted)";

            txtAddressLine.Text = r["AddressLine"] == DBNull.Value ? "" : r["AddressLine"].ToString();
            txtCity.Text = r["City"] == DBNull.Value ? "" : r["City"].ToString();
            txtPostcode.Text = r["Postcode"] == DBNull.Value ? "" : r["Postcode"].ToString();
            txtState.Text = r["State"] == DBNull.Value ? "" : r["State"].ToString();
            txtCountry.Text = r["Country"] == DBNull.Value ? "" : r["Country"].ToString();
            txtLat.Text = r["Latitude"] == DBNull.Value ? "" : Convert.ToDecimal(r["Latitude"]).ToString(CultureInfo.InvariantCulture);
            txtLng.Text = r["Longitude"] == DBNull.Value ? "" : Convert.ToDecimal(r["Longitude"]).ToString(CultureInfo.InvariantCulture);

            // Selected amenities
            DataTable dtA = Db.Query(@"
                SELECT AmenityId
                FROM SpaceAmenities
                WHERE SpaceId=@S
            ", new SqlParameter("@S", spaceId));

            var set = new HashSet<string>(dtA.AsEnumerable().Select(x => x["AmenityId"].ToString()));
            foreach (System.Web.UI.WebControls.ListItem li in cblAmenities.Items)
                li.Selected = set.Contains(li.Value);
        }

        // =========================
        // SAVE (stays on this page)
        // =========================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!TrySaveListing(out int spaceId))
                return;

            lblMsg.Text = "<div class='alert success'>Listing saved.</div>";
            LoadPhotosGrid(spaceId);

            // if it was a NEW listing, bring them into edit mode (so uploads work nicely)
            if (EditingSpaceId <= 0)
                Response.Redirect("HostCreateSpace.aspx?id=" + spaceId, false);
        }

        // ==========================================
        // COMPLETE (save + go to confirmation page)
        // ==========================================
        protected void btnComplete_Click(object sender, EventArgs e)
        {
            if (!TrySaveListing(out int spaceId))
                return;

            // ✅ Go to read-only confirmation page
            // You will create ConfirmSpace.aspx (or reuse your existing one)
            Response.Redirect("ConfirmSpace.aspx?id=" + spaceId, false);
        }

        // Save logic shared by Save + Complete
        private bool TrySaveListing(out int savedSpaceId)
        {
            savedSpaceId = 0;
            lblMsg.Text = "";

            string name = (txtName.Text ?? "").Trim();
            string location = (ddlLocation.SelectedValue ?? "").Trim();
            string type = (ddlType.SelectedValue ?? "").Trim();
            string desc = (txtDesc.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                lblMsg.Text = "<div class='alert error'>Name is required.</div>";
                return false;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity < 1)
            {
                lblMsg.Text = "<div class='alert error'>Capacity must be at least 1.</div>";
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                if (!decimal.TryParse(txtPrice.Text, out price))
                {
                    lblMsg.Text = "<div class='alert error'>Invalid price.</div>";
                    return false;
                }
            }
            if (price < 0) price = 0;

            // Address
            string addrLine = NullIfEmpty(txtAddressLine.Text);
            string city = NullIfEmpty(txtCity.Text);
            string state = NullIfEmpty(txtState.Text);
            string postcode = NullIfEmpty(txtPostcode.Text);
            string country = NullIfEmpty(txtCountry.Text);

            decimal? lat = TryParseDecimal(txtLat.Text);
            decimal? lng = TryParseDecimal(txtLng.Text);

            // UPDATE
            if (EditingSpaceId > 0)
            {
                int affected = Db.Execute(@"
                    UPDATE Spaces
                    SET Name=@Name,
                        Location=@Location,
                        Type=@Type,
                        Description=@Desc,
                        PricePerHour=@Price,
                        Capacity=@Cap,
                        AddressLine=@AddrLine,
                        City=@City,
                        State=@State,
                        Postcode=@Postcode,
                        Country=@Country,
                        Latitude=@Lat,
                        Longitude=@Lng
                    WHERE SpaceId=@S AND HostUserId=@U AND IsDeleted=0
                ",
                new SqlParameter("@Name", name),
                new SqlParameter("@Location", location),
                new SqlParameter("@Type", type),
                new SqlParameter("@Desc", string.IsNullOrWhiteSpace(desc) ? (object)DBNull.Value : desc),
                new SqlParameter("@Price", price),
                new SqlParameter("@Cap", capacity),

                new SqlParameter("@AddrLine", (object)addrLine ?? DBNull.Value),
                new SqlParameter("@City", (object)city ?? DBNull.Value),
                new SqlParameter("@State", (object)state ?? DBNull.Value),
                new SqlParameter("@Postcode", (object)postcode ?? DBNull.Value),
                new SqlParameter("@Country", (object)country ?? DBNull.Value),
                new SqlParameter("@Lat", (object)lat ?? DBNull.Value),
                new SqlParameter("@Lng", (object)lng ?? DBNull.Value),

                new SqlParameter("@S", EditingSpaceId),
                new SqlParameter("@U", UserId));

                if (affected <= 0)
                {
                    lblMsg.Text = "<div class='alert error'>Update failed.</div>";
                    return false;
                }

                CurrentSpaceId = EditingSpaceId;
                SaveAmenities(CurrentSpaceId);

                savedSpaceId = CurrentSpaceId;
                return true;
            }

            // CREATE as Draft (Option A)
            object newIdObj = Db.Scalar(@"
                INSERT INTO Spaces (HostUserId, Name, Location, Type, Description, PricePerHour, Capacity, Status, IsDeleted,
                                    AddressLine, City, State, Postcode, Country, Latitude, Longitude)
                OUTPUT INSERTED.SpaceId
                VALUES (@U, @Name, @Location, @Type, @Desc, @Price, @Cap, 'Draft', 0,
                        @AddrLine, @City, @State, @Postcode, @Country, @Lat, @Lng)
            ",
            new SqlParameter("@U", UserId),
            new SqlParameter("@Name", name),
            new SqlParameter("@Location", location),
            new SqlParameter("@Type", type),
            new SqlParameter("@Desc", string.IsNullOrWhiteSpace(desc) ? (object)DBNull.Value : desc),
            new SqlParameter("@Price", price),
            new SqlParameter("@Cap", capacity),

            new SqlParameter("@AddrLine", (object)addrLine ?? DBNull.Value),
            new SqlParameter("@City", (object)city ?? DBNull.Value),
            new SqlParameter("@State", (object)state ?? DBNull.Value),
            new SqlParameter("@Postcode", (object)postcode ?? DBNull.Value),
            new SqlParameter("@Country", (object)country ?? DBNull.Value),
            new SqlParameter("@Lat", (object)lat ?? DBNull.Value),
            new SqlParameter("@Lng", (object)lng ?? DBNull.Value)
            );

            int newSpaceId = Convert.ToInt32(newIdObj);
            CurrentSpaceId = newSpaceId;

            SaveAmenities(CurrentSpaceId);

            lblStatus.Text = "Draft";
            lblStatusHint.Text = "(Draft: not visible in Search until submitted)";
            lblTitle.Text = "Edit listing";

            savedSpaceId = CurrentSpaceId;
            return true;
        }

        // =========================
        // PHOTOS
        // =========================
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            int spaceId = (EditingSpaceId > 0) ? EditingSpaceId : CurrentSpaceId;
            if (spaceId <= 0)
            {
                lblMsg.Text = "<div class='alert error'>Please save the listing first before uploading photos.</div>";
                return;
            }

            if (!HostOwnsSpace(spaceId))
            {
                lblMsg.Text = "<div class='alert error'>Not found / not your listing.</div>";
                return;
            }

            if (fuPhotos == null || !fuPhotos.HasFiles)
            {
                lblMsg.Text = "<div class='alert error'>Please choose at least 1 photo.</div>";
                return;
            }

            // Store under app-relative folder
            string relFolder = "~/uploads/spaces/" + spaceId + "/";
            string absFolder = Server.MapPath(relFolder);
            Directory.CreateDirectory(absFolder);

            object hasCoverObj = Db.Scalar(@"SELECT COUNT(*) FROM SpacePhotos WHERE SpaceId=@S AND IsCover=1",
                new SqlParameter("@S", spaceId));
            bool hasCover = Convert.ToInt32(hasCoverObj) > 0;

            int inserted = 0;

            foreach (HttpPostedFile file in fuPhotos.PostedFiles)
            {
                if (file == null || file.ContentLength <= 0) continue;

                string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!(ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp"))
                    continue;

                string safeName = Guid.NewGuid().ToString("N") + ext;
                string relPath = relFolder + safeName;
                string absPath = Path.Combine(absFolder, safeName);

                file.SaveAs(absPath);

                bool makeCover = !hasCover && inserted == 0;

                Db.Execute(@"
                    INSERT INTO SpacePhotos (SpaceId, PhotoUrl, SortOrder, IsCover)
                    VALUES (@S, @Url, 0, @Cover)
                ",
                new SqlParameter("@S", spaceId),
                new SqlParameter("@Url", relPath),
                new SqlParameter("@Cover", makeCover ? 1 : 0));

                if (makeCover) hasCover = true;
                inserted++;
            }

            lblMsg.Text = inserted > 0
                ? "<div class='alert success'>Photos uploaded.</div>"
                : "<div class='alert error'>No valid photos uploaded (jpg/png/webp only).</div>";

            LoadPhotosGrid(spaceId);
        }

        private void LoadPhotosGrid(int spaceId)
        {
            DataTable dt = Db.Query(@"
                SELECT PhotoId, PhotoUrl, IsCover, SortOrder
                FROM SpacePhotos
                WHERE SpaceId=@S
                ORDER BY IsCover DESC, SortOrder ASC, PhotoId ASC
            ", new SqlParameter("@S", spaceId));

            if (!dt.Columns.Contains("DisplayUrl"))
                dt.Columns.Add("DisplayUrl", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                string raw = (row["PhotoUrl"] ?? "").ToString().Trim();
                row["DisplayUrl"] = ResolveUrl(NormalizeAppRelative(raw));
            }

            pnlExistingPhotos.Visible = dt.Rows.Count > 0;
            rptPhotos.DataSource = dt;
            rptPhotos.DataBind();
        }

        protected void rptPhotos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int spaceId = (EditingSpaceId > 0) ? EditingSpaceId : CurrentSpaceId;
            if (spaceId <= 0) return;
            if (!HostOwnsSpace(spaceId)) return;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int photoId))
                return;

            if (e.CommandName == "SetCover")
            {
                Db.Execute("UPDATE SpacePhotos SET IsCover=0 WHERE SpaceId=@S",
                    new SqlParameter("@S", spaceId));

                Db.Execute("UPDATE SpacePhotos SET IsCover=1 WHERE PhotoId=@P AND SpaceId=@S",
                    new SqlParameter("@P", photoId),
                    new SqlParameter("@S", spaceId));

                lblMsg.Text = "<div class='alert success'>Cover photo updated.</div>";
            }
            else if (e.CommandName == "DeletePhoto")
            {
                object urlObj = Db.Scalar("SELECT PhotoUrl FROM SpacePhotos WHERE PhotoId=@P AND SpaceId=@S",
                    new SqlParameter("@P", photoId),
                    new SqlParameter("@S", spaceId));

                int affected = Db.Execute("DELETE FROM SpacePhotos WHERE PhotoId=@P AND SpaceId=@S",
                    new SqlParameter("@P", photoId),
                    new SqlParameter("@S", spaceId));

                if (affected > 0 && urlObj != null)
                {
                    string rel = NormalizeAppRelative(urlObj.ToString());
                    try
                    {
                        string abs = Server.MapPath(rel);
                        if (File.Exists(abs)) File.Delete(abs);
                    }
                    catch { /* ignore */ }

                    object coverCount = Db.Scalar("SELECT COUNT(*) FROM SpacePhotos WHERE SpaceId=@S AND IsCover=1",
                        new SqlParameter("@S", spaceId));
                    if (Convert.ToInt32(coverCount) == 0)
                    {
                        object firstPhoto = Db.Scalar(@"
                            SELECT TOP 1 PhotoId FROM SpacePhotos WHERE SpaceId=@S ORDER BY PhotoId ASC
                        ", new SqlParameter("@S", spaceId));

                        if (firstPhoto != null)
                        {
                            Db.Execute("UPDATE SpacePhotos SET IsCover=1 WHERE PhotoId=@P",
                                new SqlParameter("@P", Convert.ToInt32(firstPhoto)));
                        }
                    }
                }

                lblMsg.Text = affected > 0
                    ? "<div class='alert success'>Photo deleted.</div>"
                    : "<div class='alert error'>Delete failed.</div>";
            }

            LoadPhotosGrid(spaceId);
        }

        // =========================
        // AMENITIES
        // =========================
        private void SaveAmenities(int spaceId)
        {
            Db.Execute("DELETE FROM SpaceAmenities WHERE SpaceId=@S", new SqlParameter("@S", spaceId));

            var selected = cblAmenities.Items.Cast<System.Web.UI.WebControls.ListItem>()
                                .Where(x => x.Selected)
                                .Select(x => x.Value)
                                .ToList();

            foreach (var amenityIdStr in selected)
            {
                if (!int.TryParse(amenityIdStr, out int amenityId)) continue;

                Db.Execute(@"
                    INSERT INTO SpaceAmenities (SpaceId, AmenityId)
                    VALUES (@S, @A)
                ",
                new SqlParameter("@S", spaceId),
                new SqlParameter("@A", amenityId));
            }
        }

        // =========================
        // HELPERS
        // =========================
        private string NullIfEmpty(string s)
        {
            s = (s ?? "").Trim();
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        private decimal? TryParseDecimal(string s)
        {
            s = (s ?? "").Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;

            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal d))
                return d;

            if (decimal.TryParse(s, out d))
                return d;

            return null;
        }

        // turn "uploads/.." or "/uploads/.." into "~/" format, and keep "http..." untouched
        private string NormalizeAppRelative(string url)
        {
            url = (url ?? "").Trim();
            if (string.IsNullOrWhiteSpace(url)) return "";

            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return url;

            if (url.StartsWith("~/")) return url;
            if (url.StartsWith("/")) return "~" + url;

            // "uploads/..."
            return "~/" + url;
        }
    }
}
