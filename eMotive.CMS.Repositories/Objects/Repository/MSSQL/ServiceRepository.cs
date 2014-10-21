using System.Data;
using System.Data.SqlClient;
using eMotive.CMS.Repositories.Interfaces;

namespace eMotive.CMS.Repositories.Objects.Repository.MSSQL
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public ServiceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal IDbConnection Connection
        {
            get
            {
                //return _connection ?? (_connection = new SqlConnection(_connectionString));
                return new SqlConnection(_connectionString);
            }
        }
    }
}
