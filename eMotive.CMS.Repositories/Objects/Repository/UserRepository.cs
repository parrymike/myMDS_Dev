using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Transactions;
using Dapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Repositories.Objects.Users;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Repositories.Objects.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly string _userFields;
        private IDbConnection _connection;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
            _userFields = "`id`, `username`, `forename`, `surname`, `email`, `enabled`, `archived`";
        }

        internal IDbConnection Connection
        {
            get
            {
                return _connection ?? new MySqlConnection(_connectionString);
            }
        }

        public User New()
        {
            return new User();
        }

        public User Fetch(int id)
        {
            using (var cn = Connection)
            {
                var sql = string.Format("SELECT {0} FROM `users` WHERE `id`=@id;", _userFields);

                var user = cn.Query<User>(sql, new { id = id }).SingleOrDefault();

                if (user != null)
                {
                    sql =
                        "SELECT b.* FROM `userHasRoles` a INNER JOIN `roles` b ON a.`roleID`=b.`id` WHERE a.`userId`=@userId";

                    user.Roles = cn.Query<Role>(sql, new { userId = id });
                }

                return user;
            }
        }

        public User Fetch(string value, FetchByUserField field)
        {
            using (var cn = Connection)
            {
                var sql = string.Format("SELECT {0} FROM `users` WHERE `{1}`=@val;", _userFields, field);

                var user = cn.Query<User>(sql, new { val = value }).SingleOrDefault();

                if (user != null)
                {
                    sql =
                        "SELECT b.* FROM `userHasRoles` a INNER JOIN `roles` b ON a.`roleID`=b.`id` WHERE a.`userId`=@userId;";

                    user.Roles = cn.Query<Role>(sql, new { userId = user.ID });
                }

                return user;
            }
        }

        public string FetUserNotes(string username)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `notes` FROM `users` WHERE `username`=@username;";

                var user = cn.Query<string>(sql, new { username = username }).SingleOrDefault();

                return user;
            }
        }

        public bool SaveUserNotes(string username, string notes)
        {
            using (var cn = Connection)
            {
                const string sql = "UPDATE  `users` SET `notes`=@notes WHERE `username`=@username;";

                return cn.Execute(sql, new { notes = notes, username = username }) > 0;
            }
        }

        public IEnumerable<User> FetchAll()
        {
            using (var cn = Connection)
            {
                var sql = string.Format("SELECT {0} FROM `users` WHERE Archived=0;", _userFields);

                var users = cn.Query<User>(sql);

                if (users.HasContent())
                {
                    //var userDict = users.ToDictionary(k => k.ID, v => v);

                    sql = "SELECT a.`userId`, b.* FROM `userHasRoles` a INNER JOIN `roles` b ON a.`roleID`=b.`id` WHERE a.`userId` IN @ids;";

                    var roles = cn.Query<RoleMap>(sql, new { ids = users.Select(n => n.ID) });

                    if (roles.HasContent())
                    {
                        var rolesUserDict = new Dictionary<int, ICollection<Role>>();

                        foreach (var item in roles)
                        {
                            ICollection<Role> currentList;

                            if (!rolesUserDict.TryGetValue(item.UserId, out currentList))
                            {
                                rolesUserDict.Add(item.UserId, new Collection<Role>());
                                rolesUserDict[item.UserId].Add(new Role { ID = item.ID, Name = item.Name});

                            }
                            else
                            {
                                rolesUserDict[item.UserId].Add(new Role { ID = item.ID, Name = item.Name});
                            }
                        }

                        foreach (var user in users)
                        {
                            user.Roles = rolesUserDict[user.ID].Distinct();
                        }
                    }
                }

                return users;
            }
        }

        private class RoleMap
        {
            public int UserId { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public IEnumerable<User> Fetch(IEnumerable<int> ids)
        {
            if (!ids.HasContent())
                return null;

            using (var cn = Connection)
            {
                var sql = string.Format("SELECT {0} FROM `users` WHERE `id` IN @ids  AND Archived=0;", _userFields);

                var users = cn.Query<User>(sql, new { ids = ids });

                if (users.HasContent())
                {
                    // var userDict = users.ToDictionary(k => k.ID, v => v);

                    sql = "SELECT a.`userId`, b.* FROM `userHasRoles` a INNER JOIN `roles` b ON a.`roleID`=b.`id` WHERE a.`userId` IN @ids;";

                    var roles = cn.Query<RoleMap>(sql, new { ids = users.Select(n => n.ID) });

                    if (roles.HasContent())
                    {
                        var rolesUserDict = new Dictionary<int, ICollection<Role>>();

                        foreach (var item in roles)
                        {
                            ICollection<Role> currentList;

                            if (!rolesUserDict.TryGetValue(item.UserId, out currentList))
                            {
                                rolesUserDict.Add(item.UserId, new Collection<Role>());
                                rolesUserDict[item.UserId].Add(new Role { ID = item.ID, Name = item.Name});

                            }
                            else
                            {
                                rolesUserDict[item.UserId].Add(new Role { ID = item.ID, Name = item.Name});
                            }
                        }

                        foreach (var user in users)
                        {
                            user.Roles = rolesUserDict[user.ID].Distinct();
                        }
                    }
                }

                return users;
            }
        }

        public IEnumerable<User> Fetch(IEnumerable<string> usernames)
        {
            if (!usernames.HasContent())
                return null;

            using (var cn = Connection)
            {
                var sql = string.Format("SELECT {0} FROM `users` WHERE `username` IN @usernames AND Archived=0;", _userFields);

                var users = cn.Query<User>(sql, new { usernames = usernames });

                if (users.HasContent())
                {
                    // var userDict = users.ToDictionary(k => k.ID, v => v);

                    sql = "SELECT a.`userId`, b.* FROM `userHasRoles` a INNER JOIN `roles` b ON a.`roleID`=b.`id` WHERE a.`userId` IN @ids;";

                    var roles = cn.Query<RoleMap>(sql, new { ids = users.Select(n => n.ID) });

                    if (roles.HasContent())
                    {
                        var rolesUserDict = new Dictionary<int, ICollection<Role>>();

                        foreach (var item in roles)
                        {
                            ICollection<Role> currentList;

                            if (!rolesUserDict.TryGetValue(item.UserId, out currentList))
                            {
                                rolesUserDict.Add(item.UserId, new Collection<Role>());
                                rolesUserDict[item.UserId].Add(new Role { ID = item.ID, Name = item.Name});

                            }
                            else
                            {
                                rolesUserDict[item.UserId].Add(new Role { ID = item.ID, Name = item.Name});
                            }
                        }

                        foreach (var user in users)
                        {
                            user.Roles = rolesUserDict[user.ID].Distinct();
                        }
                    }
                }

                return users;
            }
        }

        public bool Create(User user, out int id)
        {
            using (var connection = Connection)
            {
                using (var transactionScope = new TransactionScope())
                {
                    connection.Open();
                    var sql = "INSERT INTO `users` (`username`, `forename`, `surname`, `email`, `enabled`, `archived`) VALUES (@username, @forename, @surname, @email, @enabled, @archived);";

                    var cn = connection.Execute(sql, new
                    {
                        username = user.Username,
                        forename = user.Forename,
                        surname = user.Surname,
                        email = user.Email,
                        enabled = user.Enabled,
                        archived = user.Archived
                    }) > 0;

                    var insertedID = connection.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                    id = Convert.ToInt32(insertedID);
                    var insertObj = user.Roles.Select(n => new { UserId = insertedID, RoleId = n.ID });
                    sql = "INSERT INTO `UserHasRoles` (`UserID`, `RoleId`) VALUES (@UserId, @RoleId);";
                    cn = connection.Execute(sql, insertObj) > 0;

                    transactionScope.Complete();

                    return cn & insertedID > 0;
                }
            }
        }

        public bool Update(User user)
        {
            using (var cn = Connection)
            {
                using (var transactionScope = new TransactionScope())
                {
                    cn.Open();

                    var sql = "UPDATE `users` SET `username` = @username, `forename` = @forename, `surname`= @surname, `email` = @email, `enabled` = @enabled, `archived` = @archived WHERE `id` = @id";

                    var success = cn.Execute(sql, new
                    {
                        username = user.Username,
                        forename = user.Forename,
                        surname = user.Surname,
                        email = user.Email,
                        enabled = user.Enabled,
                        archived = user.Archived,
                        id = user.ID
                    }) > 0;

                    //todo: how to only alter roles which have been updated?

                    sql = "SELECT `RoleID` FROM `UserHasRoles` WHERE `UserID` = @UserId";

                    var oldRoles = cn.Query<int>(sql, new { UserId = user.ID });

                    var enumerable = oldRoles as int[] ?? oldRoles.ToArray();
                    //  if (!enumerable.HasContent())
                    //  {
                    //   sql = "INSERT INTO `UserHasRoles` (`UserID`, `RoleId`) VALUES (@UserId, @RoleId);";
                    //   var insertObj = _user.Roles.Select(n => new { UserId = _user.ID, RoleId = n.ID });
                    //   success = _connection.Execute(sql, insertObj) > 0;
                    // }
                    // else
                    // {var toDelete = enumerable.Where(n => !_user.Roles.Any(m => m.ID == n));
                    var toDelete = enumerable.Where(n => user.Roles.All(m => m.ID != n));
                    var toUpdate = user.Roles.Where(n => !enumerable.Any(m => m == n.ID));

                    var delete = toDelete as int[] ?? toDelete.ToArray();
                    if (delete.HasContent())
                    {
                        sql = "DELETE FROM `UserHasRoles` WHERE `RoleId` IN @roleIds AND `UserId` = @UserId;";

                        success = cn.Execute(sql, new { roleIds = delete, UserId = user.ID }) > 0;
                    }

                    var update = toUpdate as Role[] ?? toUpdate.ToArray();
                    if (update.HasContent())
                    {
                        sql = "INSERT INTO `UserHasRoles` (`UserID`, `RoleId`) VALUES (@UserId, @RoleId);";
                        var insertObj = update.Select(n => new { UserId = user.ID, RoleId = n.ID });
                        success = cn.Execute(sql, insertObj) > 0;
                    }
                    // }

                    transactionScope.Complete();

                    return success;
                }
            }
        }

        public bool Delete(User user)
        {
            user.Enabled = false;
            user.Surname = string.Empty;
            user.Forename = string.Empty;
            user.Archived = false;

            using (var cn = Connection)
            {
                const string sql = "UPDATE `users` SET `username` = @username, `forename` = @forename, `surname`= @surname, `email` = @email, `enabled` = @enabled, `archived` = @archived WHERE `id` = @id";
                var sqlParams = new
                {
                    username = user.Username,
                    forename = string.Empty,
                    surname = string.Empty,
                    email = user.Email,
                    enabled = false,
                    archived = true,
                    id = user.ID
                };

                return cn.Execute(sql, sqlParams) > 0;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (var cn = Connection)
            {
                cn.Open();

                const string sql = "SELECT CAST(Count(`id`) AS UNSIGNED INTEGER) FROM `users` WHERE `username`=@username AND `password`=@password;";

                return cn.Query<ulong>(sql, new { username = username, password = password }).SingleOrDefault() > 0;
            }
        }

        public bool SavePassword(int _id, string _salt, string _password)
        {
            using (var cn = Connection)
            {
                cn.Open();

                const string sql = "UPDATE `users` SET `salt` = @salt, `password`=@password WHERE `id`=@id;";

                var success = cn.Execute(sql, new { id = _id, salt = _salt, password = _password }) > 0;

                return success;
            }
        }

        public string GetSalt(string _username)
        {
            using (var cn = Connection)
            {
                cn.Open();

                const string sql = "SELECT `salt` FROM `users` WHERE `username`=@username;";

                return cn.Query<string>(sql, new { username = _username }).SingleOrDefault();
            }
        }
    }
}
