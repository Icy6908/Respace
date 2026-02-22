using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class SupportTicket : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) Response.Redirect("Login.aspx");

            if (!IsPostBack)
            {
                LoadHistory();
            }
        }

        private void LoadHistory()
        {
            string userId = Session["UserId"].ToString();

            
            string sql = "SELECT * FROM SupportQueries WHERE UserId = @uid ORDER BY SubmittedAt DESC";
            DataTable dt = Db.Query(sql, new SqlParameter("@uid", userId));
            rptHistory.DataSource = dt;
            rptHistory.DataBind();

            
            string updateSql = "UPDATE SupportQueries SET IsRead = 1 WHERE UserId = @uid AND AdminReply IS NOT NULL";
            Db.Query(updateSql, new SqlParameter("@uid", userId));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string userId = Session["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId)) return;

            string attachmentPath = "";
            if (fileAttachment.HasFile)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + fileAttachment.FileName;
                attachmentPath = "~/Uploads/" + fileName;
                fileAttachment.SaveAs(Server.MapPath(attachmentPath));
            }

           
            string query = @"INSERT INTO SupportQueries (UserId, Subject, Message, Status, SubmittedAt, AttachmentUrl, IsRead) 
                             VALUES (@uid, @sub, @msg, 'Pending', GETDATE(), @attach, 0)";

            SqlParameter[] ps = {
                new SqlParameter("@uid", userId),
                new SqlParameter("@sub", txtSubject.Text),
                new SqlParameter("@msg", txtMessage.Text),
                new SqlParameter("@attach", attachmentPath)
            };

            Db.Query(query, ps);

            lblStatus.Text = "✓ Ticket Submitted Successfully";
            lblStatus.ForeColor = System.Drawing.Color.SeaGreen;

            txtSubject.Text = "";
            txtMessage.Text = "";
            LoadHistory(); 
        }
    }
}