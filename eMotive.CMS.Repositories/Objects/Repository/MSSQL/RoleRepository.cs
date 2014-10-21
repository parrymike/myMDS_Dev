using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Repositories.Objects.Users;


namespace eMotive.CMS.Repositories.Objects.Repository.MSSQL
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public RoleRepository(string connectionString)
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

        public Role New()
        {
            return new Role();
        }

        public Role Fetch(int id)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `Name` FROM `roles` WHERE `id`=@id;";

                return cn.Query<Role>(sql, new { id = id }).SingleOrDefault();
            }
        }

        public Role Fetch(string name)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `Name` FROM `roles` WHERE `Name`=@name;";

                return cn.Query<Role>(sql, new { name = name }).SingleOrDefault();
            }
        }

        public IEnumerable<Role> Fetch(IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `Name` FROM `roles` WHERE `id` IN @ids;";

                return cn.Query<Role>(sql, new { ids = ids });
            }
        }

        public IEnumerable<Role> Fetch(IEnumerable<string> names)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `Name` FROM `roles` WHERE `Name` IN @names;";

                return cn.Query<Role>(sql, new { names = names });
            }
        }

        public IEnumerable<Role> FetchAll()
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `Name` FROM `roles`;";

                return cn.Query<Role>(sql);
            }
        }

        //http://stackoverflow.com/questions/6387904/how-to-insert-an-ienumerablet-collection-with-dapper-dot-net
        public bool AddUserToRoles(int _id, IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                const string sql = "INSERT INTO `UserHasRoles` (`UserId`, `RoleId`) VALUES (@idUser, @idRole);";

                return cn.Execute(sql, new { Enumerable = ids.Select(n => new { idUser = _id, idRole = n }) }) > 0;
            }
        }

        public bool RemoveUserFromRoles(int _userId, IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                const string sql = "DELETE FROM `UserHasRoles` WHERE `UserId` = @idUser AND `RoleId` = @idRole);";

                return cn.Execute(sql, new { Enumerable = ids.Select(n => new { idUser = _userId, idRole = n }) }) > 0;
            }
        }

        public IEnumerable<int> FindUsersInRole(int id)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `UserId` FROM `UserHasRoles` WHERE `RoleId` = @id;";

                return cn.Query<int>(sql, new { id = id });
            }
        }

        public IEnumerable<Role> FetchUserRoles(int userId)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT a.`id`, a.`Name` FROM `roles` a INNER JOIN `UserHasRoles` b ON a.`id`=b.`idRole` WHERE b.`idUser` = @idUser;";

                return cn.Query<Role>(sql, new { idUser = userId });
            }
        }

        public bool Update(Role role)
        {
            using (var cn = Connection)
            {
                const string sql = "UPDATE `roles` SET `Name`= @name WHERE `id` = @id;";

                return cn.Execute(sql, new { name = role.Name, id = role.ID }) > 0;
            }
        }

        public bool Create(Role role)
        {
            using (var cn = Connection)
            {
                const string sql = "INSERT INTO `roles` (`Name`) VALUES (@name);";

                var success = cn.Execute(sql, new { name = role.Name }) > 0;

                return success;
            }
        }

        public bool Delete(string role)
        {
            using (var cn = Connection)
            {
                const string sql = "DELETE FROM `roles WHERE `Name`=@name;";

                return cn.Execute(sql, new { name = role }) > 0;
            }
        }

        public bool Delete(int id)
        {
            using (var cn = Connection)
            {
                const string sql = "DELETE FROM `roles` WHERE `id`=@id;";

                return cn.Execute(sql, new { id = id }) > 0;
            }
        }
    }
}
