using Respace.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Respace.Admin
{
    public partial class ManageCoupons : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Pulling directly from the UserCoupons table shown in your schema
                gvUserCoupons.DataSource = Db.Query("SELECT * FROM UserCoupons ORDER BY RedeemedAt DESC");
                gvUserCoupons.DataBind();
            }
        }
    }
}