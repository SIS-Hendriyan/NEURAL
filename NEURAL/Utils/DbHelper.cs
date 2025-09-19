using System.Data.SqlClient;

namespace NEURAL.Utils
{
    public class DbHelper
    {
        private static readonly string _connectionString = Startup.connectionstring;
        public static SqlConnection GetOpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }

}
