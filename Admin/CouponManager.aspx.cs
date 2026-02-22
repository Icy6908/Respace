using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Respace.App_Code;

namespace Respace.Admin
{
    public partial class CouponManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
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
            string query = "SELECT * FROM CouponDefinitions WHERE IsActive = 1 ORDER BY PointCost ASC";
            gvStore.DataSource = Db.Query(query);
            gvStore.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
   
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtAmount.Text) || string.IsNullOrWhiteSpace(txtCost.Text))
            {
                lblStatus.Text = "Please fill in all fields.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
           
                string sql = @"INSERT INTO CouponDefinitions (CouponCode, DiscountAmount, PointCost, IsActive) 
                               VALUES (@Code, @Amt, @Cost, 1)";

                SqlParameter[] parameters = {
                    new SqlParameter("@Code", txtCode.Text.Trim().ToUpper()),
                    new SqlParameter("@Amt", decimal.Parse(txtAmount.Text.Trim())),
                    new SqlParameter("@Cost", int.Parse(txtCost.Text.Trim()))
                };

                Db.Execute(sql, parameters);

            
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
        
            int defId = Convert.ToInt32(gvStore.DataKeys[e.RowIndex].Value);

       
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