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
            // Ensure database objects required by the app exist in the configured database.
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
                // Fail fast during development so missing DB objects are discovered early.
                System.Diagnostics.Trace.TraceError("Database initialization failed: " + ex.Message);
            }
        }
    }
}
