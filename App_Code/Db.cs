using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Respace.App_Code
{
    public static class Db
    {
        private static string ConnStr
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["RespaceDb"].ConnectionString;
            }
        }

        public static DataTable Query(string sql, params SqlParameter[] p)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (p != null) cmd.Parameters.AddRange(p);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int Execute(string sql, params SqlParameter[] p)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (p != null) cmd.Parameters.AddRange(p);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Scalar(string sql, params SqlParameter[] p)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (p != null) cmd.Parameters.AddRange(p);
                con.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
