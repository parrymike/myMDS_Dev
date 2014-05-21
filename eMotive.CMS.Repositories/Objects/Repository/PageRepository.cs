using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using Dapper;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Repositories.Objects.Pages;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Repositories.Objects.Repository
{
    public class PageRepository : IPageRepository
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public PageRepository(string connectionString)
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

        public Page New()
        {
            return new Page();
        }

        public bool Create(Page page, out int id)
        {
            throw new System.NotImplementedException();
        }

        public bool Put(Page page)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(Page page)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(Page page)
        {
            throw new System.NotImplementedException();
        }

        public Page FetchPage(int id)
        {
            throw new System.NotImplementedException();
        }

        public Page FetchPage(string name)
        {
            throw new System.NotImplementedException();
        }

        public Section NewSection()
        {
            return new Section();
        }

        public bool Create(Section section, out int id)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();

                    var success = true;
                    id = -1;

                    var sql = "INSERT INTO `pagesections` (`Name`) VALUES (@Name);";

                    success = cn.Execute(sql, section) > 0;

                    var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                    id = Convert.ToInt32(newId);

                    return success;
                }
            }
        }

        public bool Put(Section section)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();
                    var success = true;

                    var sql = "DELETE FROM `pagesections` WHERE `id`=@id;";
                    success &= cn.Execute(sql, new { id = section.ID }) > 0;

                    if (success)
                    {
                        sql = "INSERT INTO `pagesections` (`ID`, `Name`) VALUES (@ID, @Name);";
                        success &= cn.Execute(sql, section) > 0;
                    }

                    if (!success)
                        transaction.Dispose();
                    else
                        transaction.Complete();



                    return success;
                }
            }

        }

        public bool Update(Section section)
        {
            using (var cn = Connection)
            {
                var sql = "UPDATE `pagesections` SET `Name`=@Name WHERE `id`=@id;";

                return cn.Execute(sql, section) > 0;
            }
        }

        public bool Delete(Section section)
        {
            using (var cn = Connection)
            {
                var sql = "DELETE FROM `pagesections` WHERE `id`=@id;";

                return cn.Execute(sql, section) > 0;
            }
        }

        public Section FetchSection(int id)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `pagesections` WHERE `ID`=@id;";

                return cn.Query<Section>(sql, new { id = id }).SingleOrDefault();
            }
        }

        public Section FetchSection(string name)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `pagesections` WHERE `Name`=@name;";

                return cn.Query<Section>(sql, new { name = name }).SingleOrDefault();
            }
        }

        public IEnumerable<Section> Fetch()
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `pagesections`;";

                return cn.Query<Section>(sql);
            }
        }

        public IEnumerable<Section> Fetch(IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `pagesections` WHERE `id` IN @ids;";

                return cn.Query<Section>(sql, new { ids = ids });
            }
        }
    }
}
