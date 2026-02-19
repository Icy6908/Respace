using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Respace.App_Code; // Ensure this matches your namespace for the Db class

namespace Respace.Admin
{
    public partial class CouponManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Security check for Admin role
            if (Session["Role"]?.ToString() != "Admin")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindStoreGrid();
            }
        }

        private void BindStoreGrid()
        {
            // Pulls active definitions from your CouponDefinitions table
            string query = "SELECT * FROM CouponDefinitions WHERE IsActive = 1 ORDER BY PointCost ASC";
            gvStore.DataSource = Db.Query(query);
            gvStore.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Validation to ensure data is present before SQL execution
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtAmount.Text) || string.IsNullOrWhiteSpace(txtCost.Text))
            {
                lblStatus.Text = "Please fill in all fields.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                // Inserting into the specific columns identified in your schema
                string sql = @"INSERT INTO CouponDefinitions (CouponCode, DiscountAmount, PointCost, IsActive) 
                               VALUES (@Code, @Amt, @Cost, 1)";

                SqlParameter[] parameters = {
                    new SqlParameter("@Code", txtCode.Text.Trim().ToUpper()),
                    new SqlParameter("@Amt", decimal.Parse(txtAmount.Text.Trim())),
                    new SqlParameter("@Cost", int.Parse(txtCost.Text.Trim()))
                };

                Db.Execute(sql, parameters);

                // Success feedback and UI refresh
                lblStatus.Text = "New reward tier added successfully!";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                ClearForm();
                BindStoreGrid();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void gvStore_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Retrieves the CouponDefId from DataKeyNames
            int defId = Convert.ToInt32(gvStore.DataKeys[e.RowIndex].Value);

            // Soft-delete by setting IsActive to 0
            Db.Execute("UPDATE CouponDefinitions SET IsActive = 0 WHERE CouponDefId = @ID",
                new SqlParameter("@ID", defId));

            BindStoreGrid();
        }

        private void ClearForm()
        {
            txtCode.Text = "";
            txtAmount.Text = "";
            txtCost.Text = "";
        }
    }
}