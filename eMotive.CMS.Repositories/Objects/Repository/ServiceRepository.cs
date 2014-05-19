using System.Data;
using eMotive.CMS.Repositories.Interfaces;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Repositories.Objects.Repository
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
                return _connection ?? new MySqlConnection(_connectionString);
            }
        }
    }
}
