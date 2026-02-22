using System;
using System.IO;
using System.Data.SqlClient;
using System.Web;

namespace Respace
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
           
            try
            {
                var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["RespaceDb"].ConnectionString;
                using (var con = new SqlConnection(connStr))
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = File.ReadAllText(Server.MapPath("~/sql/CreatePointsTransactions.sql"));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
  
                System.Diagnostics.Trace.TraceError("Database initialization failed: " + ex.Message);
            }
        }
    }
}
