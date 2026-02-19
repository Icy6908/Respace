using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace
{
    public partial class SupportTicket : System.Web.UI.Page
    {
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Use the UserId stored during login
            string userId = Session["UserId"]?.ToString();

            if (string.IsNullOrEmpty(userId))
            {
                lblStatus.Text = "Please login to submit a ticket.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                // Matching your table columns exactly
                string query = @"INSERT INTO SupportQueries (UserId, Subject, Message, Status, SubmittedAt) 
                                 VALUES (@uid, @sub, @msg, 'Pending', GETDATE())";

                SqlParameter[] ps = {
                    new SqlParameter("@uid", userId),
                    new SqlParameter("@sub", txtSubject.Text),
                    new SqlParameter("@msg", txtMessage.Text)
                };

                Db.Query(query, ps);

                lblStatus.Text = "Your ticket has been submitted! Our admin will review it soon.";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                // Clear the form
                txtSubject.Text = "";
                txtMessage.Text = "";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}