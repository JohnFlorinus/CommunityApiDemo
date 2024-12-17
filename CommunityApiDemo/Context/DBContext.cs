using System.Data;
using Microsoft.Data.SqlClient;

namespace CommunityApiDemo.Context
{
    public class DBContext
    {
        private readonly string connectionString = "";

        public DBContext(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SQLPath");
        }

        public IDbConnection CreateConnection() => new SqlConnection(connectionString);
    }
}
