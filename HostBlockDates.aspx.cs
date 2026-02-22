using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Respace.App_Code;

namespace Respace
{
    public partial class HostBlockDates : System.Web.UI.Page
    {
        private int UserId => (Session["UserId"] == null) ? 0 : Convert.ToInt32(Session["UserId"]);
        private string Role => (Session["Role"] ?? "").ToString();

        private int SpaceId
        {
            get
            {
                int id;
                int.TryParse(Request.QueryString["spaceId"], out id);
                return id;
            }
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

            if (SpaceId <= 0)
            {
                lblMsg.Text = "<div class='alert error'>Invalid space.</div>";
                return;
            }

           
            if (!HostOwnsSpace())
            {
                lblMsg.Text = "<div class='alert error'>Not found / not your listing.</div>";
                return;
            }

            LoadSpaceName();
            LoadBlocks();
            LoadDisabledDatesForHostCalendar();

            if (!IsPostBack)
            {
               
            }
        }

        private bool HostOwnsSpace()
        {
            object ok = Db.Scalar(@"
                SELECT COUNT(*)
                FROM Spaces
                WHERE SpaceId=@S AND HostUserId=@U AND IsDeleted=0
            ",
            new SqlParameter("@S", SpaceId),
            new SqlParameter("@U", UserId));

            return Convert.ToInt32(ok) > 0;
        }

        private void LoadSpaceName()
        {
            object name = Db.Scalar("SELECT Name FROM Spaces WHERE SpaceId=@S",
                new SqlParameter("@S", SpaceId));

            lblSpaceName.Text = (name ?? "").ToString();
        }

        private void LoadBlocks()
        {
            DataTable dt = Db.Query(@"
                SELECT BlockId, StartDate, EndDate, Reason
                FROM SpaceBlocks
                WHERE SpaceId=@S AND IsActive=1
                ORDER BY StartDate ASC
            ", new SqlParameter("@S", SpaceId));

            gvBlocks.DataSource = dt;
            gvBlocks.DataBind();
        }


        private void LoadDisabledDatesForHostCalendar()
        {
            var days = new HashSet<string>();

          
            DataTable dtB = Db.Query(@"
                SELECT StartDateTime, EndDateTime
                FROM Bookings
                WHERE SpaceId=@S
                  AND Status IN ('Confirmed','Pending')
            ", new SqlParameter("@S", SpaceId));

            foreach (DataRow row in dtB.Rows)
            {
                DateTime start = Convert.ToDateTime(row["StartDateTime"]).Date;
                DateTime end = Convert.ToDateTime(row["EndDateTime"]).Date; 
                for (DateTime d = start; d < end; d = d.AddDays(1))
                    days.Add(d.ToString("yyyy-MM-dd"));
            }


            DataTable dtBlk = Db.Query(@"
                SELECT StartDate, EndDate
                FROM SpaceBlocks
                WHERE SpaceId=@S AND IsActive=1
            ", new SqlParameter("@S", SpaceId));

            foreach (DataRow row in dtBlk.Rows)
            {
                DateTime start = Convert.ToDateTime(row["StartDate"]).Date;
                DateTime end = Convert.ToDateTime(row["EndDate"]).Date; 
                for (DateTime d = start; d < end; d = d.AddDays(1))
                    days.Add(d.ToString("yyyy-MM-dd"));
            }

            hfDisabledDates.Value = string.Join(",", days);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            if (!TryParseYmd(hfStart.Value, out DateTime start) ||
                !TryParseYmd(hfEnd.Value, out DateTime end))
            {
                lblMsg.Text = "<div class='alert error'>Please pick dates.</div>";
                LoadBlocks();
                LoadDisabledDatesForHostCalendar();
                return;
            }

            start = start.Date;
            end = end.Date;

            if (end <= start)
            {
                lblMsg.Text = "<div class='alert error'>End date must be after start date.</div>";
                LoadBlocks();
                LoadDisabledDatesForHostCalendar();
                return;
            }

         
            object overlap = Db.Scalar(@"
                SELECT
                  (SELECT COUNT(*)
                   FROM Bookings
                   WHERE SpaceId=@S
                     AND Status IN ('Confirmed','Pending')
                     AND NOT (EndDateTime <= @Start OR StartDateTime >= @End)
                  )
                  +
                  (SELECT COUNT(*)
                   FROM SpaceBlocks
                   WHERE SpaceId=@S AND IsActive=1
                     AND NOT (EndDate <= @Start OR StartDate >= @End)
                  )
            ",
            new SqlParameter("@S", SpaceId),
            new SqlParameter("@Start", start),
            new SqlParameter("@End", end));

            if (Convert.ToInt32(overlap) > 0)
            {
                lblMsg.Text = "<div class='alert error'>These dates overlap with an existing booking/block.</div>";
                LoadBlocks();
                LoadDisabledDatesForHostCalendar();
                return;
            }

            string reason = (txtReason.Text ?? "").Trim();

            Db.Execute(@"
                INSERT INTO SpaceBlocks (SpaceId, HostUserId, StartDate, EndDate, Reason, IsActive)
                VALUES (@S, @U, @Start, @End, @R, 1)
            ",
            new SqlParameter("@S", SpaceId),
            new SqlParameter("@U", UserId),
            new SqlParameter("@Start", start),
            new SqlParameter("@End", end),
            new SqlParameter("@R", string.IsNullOrWhiteSpace(reason) ? (object)DBNull.Value : reason));

            txtReason.Text = "";
            txtStart.Text = "";
            txtEnd.Text = "";
            hfStart.Value = "";
            hfEnd.Value = "";

            lblMsg.Text = "<div class='alert success'>Dates blocked.</div>";

            LoadBlocks();
            LoadDisabledDatesForHostCalendar();
        }

        protected void gvBlocks_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "Unblock", StringComparison.OrdinalIgnoreCase))
                return;

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int blockId))
                return;

            int affected = Db.Execute(@"
                UPDATE SpaceBlocks
                SET IsActive=0
                WHERE BlockId=@B AND SpaceId=@S AND HostUserId=@U
            ",
            new SqlParameter("@B", blockId),
            new SqlParameter("@S", SpaceId),
            new SqlParameter("@U", UserId));

            lblMsg.Text = affected > 0
                ? "<div class='alert success'>Dates unblocked.</div>"
                : "<div class='alert error'>Unblock failed.</div>";

            LoadBlocks();
            LoadDisabledDatesForHostCalendar();
        }

        private bool TryParseYmd(string s, out DateTime dt)
        {
            dt = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(s)) return false;

            return DateTime.TryParseExact(
                s.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dt
            );
        }
    }
}
