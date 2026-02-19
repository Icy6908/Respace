using System;
using System.Data;
using System.Data.SqlClient;
using Respace.App_Code;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class Message : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) Response.Redirect("Login.aspx");

            if (!IsPostBack)
            {
                LoadChatList("Active");
                if (Request.QueryString["bid"] != null)
                {
                    lblNoChat.Visible = false;
                    LoadMessages();
                }
                else
                {
                    lblNoChat.Visible = true;
                    phSpaceInfo.Visible = false;
                }
            }
        }

        protected void Filter_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string filter = btn.CommandArgument;

            // Toggle Active Class
            btnShowActive.CssClass = filter == "Active" ? "btn btn-sm btn-outline-secondary active" : "btn btn-sm btn-outline-secondary";
            btnShowPast.CssClass = filter == "Past" ? "btn btn-sm btn-outline-secondary active" : "btn btn-sm btn-outline-secondary";

            LoadChatList(filter);
        }

        private void LoadChatList(string status)
        {
            string uid = Session["UserId"].ToString();
            // Using StartDateTime and EndDateTime per database schema
            string dateCondition = status == "Active" ? "b.EndDateTime >= GETDATE()" : "b.EndDateTime < GETDATE()";

            string sql = $@"
                SELECT DISTINCT 
                    u.FullName as PartnerName, 
                    u.UserId as TargetUserId, 
                    b.BookingId, 
                    s.Name as SpaceName,
                    b.StartDateTime,
                    b.EndDateTime
                FROM Bookings b
                JOIN Spaces s ON b.SpaceId = s.SpaceId
                JOIN Users u ON (u.UserId = s.HostUserId OR u.UserId = b.GuestUserId)
                WHERE ((b.GuestUserId = @uid OR s.HostUserId = @uid) AND u.UserId <> @uid)
                AND {dateCondition}
                ORDER BY b.StartDateTime DESC";

            rptChatList.DataSource = Db.Query(sql, new SqlParameter("@uid", uid));
            rptChatList.DataBind();
        }

        private void LoadMessages()
        {
            string bid = Request.QueryString["bid"];
            string uid = Session["UserId"]?.ToString();

            if (string.IsNullOrEmpty(bid)) return;

            string spaceSql = @"
                SELECT s.Name, b.BookingId, b.StartDateTime, b.EndDateTime,
                (SELECT TOP 1 PhotoUrl FROM SpacePhotos WHERE SpaceId = s.SpaceId) as Img 
                FROM Bookings b 
                JOIN Spaces s ON b.SpaceId = s.SpaceId 
                WHERE b.BookingId = @bid";

            DataTable dtSpace = Db.Query(spaceSql, new SqlParameter("@bid", bid));
            if (dtSpace.Rows.Count > 0)
            {
                phSpaceInfo.Visible = true;
                litSpaceName.Text = dtSpace.Rows[0]["Name"].ToString();
                litBookingId.Text = dtSpace.Rows[0]["BookingId"].ToString();
                imgSpaceDetail.ImageUrl = dtSpace.Rows[0]["Img"].ToString();
                litStartDate.Text = Convert.ToDateTime(dtSpace.Rows[0]["StartDateTime"]).ToString("MMM dd, yyyy");
                litEndDate.Text = Convert.ToDateTime(dtSpace.Rows[0]["EndDateTime"]).ToString("MMM dd, yyyy");
            }

            string msgSql = "SELECT * FROM Messages WHERE BookingID = @bid ORDER BY Timestamp ASC";
            DataTable dtMsgs = Db.Query(msgSql, new SqlParameter("@bid", bid));

            if (dtMsgs.Rows.Count > 0)
            {
                lblNoChat.Visible = false;
                rptMessages.DataSource = dtMsgs;
                rptMessages.DataBind();

                string updateSql = "UPDATE Messages SET IsRead = 1 WHERE BookingID = @bid AND ReceiverID = @uid AND IsRead = 0";
                Db.Query(updateSql, new SqlParameter("@bid", bid), new SqlParameter("@uid", uid));
            }
            else
            {
                lblNoChat.Text = "No messages yet. Say hi!";
                lblNoChat.Visible = true;
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewMsg.Text)) return;

            string bid = Request.QueryString["bid"];
            string uid = Session["UserId"].ToString();

            string getReceiverSql = @"
                SELECT CASE WHEN b.GuestUserId = @uid THEN s.HostUserId ELSE b.GuestUserId END as ReceiverId
                FROM Bookings b JOIN Spaces s ON b.SpaceId = s.SpaceId WHERE b.BookingId = @bid";

            DataTable dtReceiver = Db.Query(getReceiverSql, new SqlParameter("@uid", uid), new SqlParameter("@bid", bid));

            if (dtReceiver.Rows.Count > 0)
            {
                string rid = dtReceiver.Rows[0]["ReceiverId"].ToString();
                string sql = "INSERT INTO Messages (SenderID, ReceiverID, BookingID, MessageText, Timestamp, IsRead) VALUES (@sid, @rid, @bid, @txt, GETDATE(), 0)";

                Db.Query(sql, new SqlParameter("@sid", uid), new SqlParameter("@rid", rid), new SqlParameter("@bid", bid), new SqlParameter("@txt", txtNewMsg.Text.Trim()));

                txtNewMsg.Text = "";
                LoadMessages();
            }
        }
    }
}