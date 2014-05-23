using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using Dapper;
using eMotive.CMS.Extensions;
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
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();

                    var success = true;
                    id = -1;

                    var sql = "INSERT INTO `pages` (`Name`, `Title`, `Text`,`PageSection`) VALUES (@Name, @Title, @Text, @PageSection);";

                    success = cn.Execute(sql, page) > 0;

                    var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                    id = Convert.ToInt32(newId);

                    transaction.Complete();

                    return success;
                }
            }
        }

        public bool Put(Page page)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();
                    var success = true;

                    var sql = "DELETE FROM `pages` WHERE `id`=@id;";
                    success &= cn.Execute(sql, new { id = page.ID }) > 0;

                    if (success)
                    {
                        sql = "INSERT INTO `pages` (`ID`, `Name`, `Title`, `Text`,`PageSection`) VALUES (@ID, @Name, @Title, @Text, @PageSection);";
                        success &= cn.Execute(sql, page) > 0;
                    }

                    if (!success)
                        transaction.Dispose();
                    else
                        transaction.Complete();



                    return success;
                }
            }
        }

        public bool Update(Page page)
        {
            using (var cn = Connection)
            {
                var sql = "UPDATE `pages` SET `Name`=@Name, `Title`=@Title, `Text`=@Text,`PageSection`=@PageSection WHERE `id`=@ID;";

                return cn.Execute(sql, page) > 0;
            }
        }

        public bool Delete(Page page)
        {
            using (var cn = Connection)
            {
                var sql = "DELETE FROM `id`=@ID;";

                return cn.Execute(sql, page.ID) > 0;
            }
        }

        public Page FetchPage(int id)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name`, `Title`, `Text`,`PageSection` FROM `Pages` WHERE `ID`=@id;";

                return cn.Query<Page>(sql, new {id = id}).SingleOrDefault();
            }
        }

        public Page FetchPage(string name)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name`, `Title`, `Text`,`PageSection` FROM `Pages` WHERE `Name`=@name;";

                return cn.Query<Page>(sql, new { name = name }).SingleOrDefault();
            }
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

                var section = cn.Query<Section>(sql, new { id = id }).SingleOrDefault();

                if (section != null)
                {
                    sql = "SELECT `ID`, `Name`, `Title` FROM `Pages` WHERE `PageSection`=@pageSection;";
                    section.Pages = cn.Query<PageProjection>(sql, new {section.ID});
                }

                return section;
            }
        }

        public Section FetchSection(string name)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `pagesections` WHERE `Name`=@name;";

                var section = cn.Query<Section>(sql, new { name = name }).SingleOrDefault();

                if (section != null)
                {
                    sql = "SELECT `ID`, `Name`, `PageSection` FROM `Pages` WHERE `PageSection`=@pageSection;";
                    section.Pages = cn.Query<PageProjection>(sql, new { section.ID });
                }

                return section;
            }
        }

        public IEnumerable<Section> FetchSections()
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `pagesections`;";

                var sections = cn.Query<Section>(sql);

                if (!sections.IsEmpty())
                {
                    sql = "SELECT `ID`, `Name`, `PageSection` FROM `Pages` WHERE `PageSection` IN @pageSections;";

                    var pageSections = cn.Query<PageProjection>(sql, new {pageSections = sections.Select(n => n.ID)});

                    if (!pageSections.IsEmpty())
                    {
                        var pageSectionDict = pageSections.GroupBy(n => n.PageSection).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var section in sections)
                        {
                            List<PageProjection> pageList;

                            if (pageSectionDict.TryGetValue(section.ID, out pageList))
                                section.Pages = pageList;
                        }
                    }
                }

                return sections;
            }
        }

        public IEnumerable<Section> FetchSections(IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `pagesections` WHERE `id` IN @ids;";

                var sections = cn.Query<Section>(sql, new { ids = ids });

                if (!sections.IsEmpty())
                {
                    sql = "SELECT `ID`, `Name`, `PageSection` FROM `Pages` WHERE `PageSection` IN @pageSections;";

                    var pageSections = cn.Query<PageProjection>(sql, new { pageSections = sections.Select(n => n.ID) });

                    if (!pageSections.IsEmpty())
                    {
                        var pageSectionDict = pageSections.GroupBy(n => n.PageSection).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var section in sections)
                        {
                            List<PageProjection> pageList;

                            if (pageSectionDict.TryGetValue(section.ID, out pageList))
                                section.Pages = pageList;
                        }
                    }
                }

                return sections;
            }
        }

        public IEnumerable<Page> FetchPages()
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name`, `Title`, `Text`,`PageSection` FROM `Pages`;";

                return cn.Query<Page>(sql);
            }
        }

        public IEnumerable<Page> FetchPages(IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name`, `Title`, `Text`,`PageSection` FROM `Pages` WHERE `ID` in @ids;";

                return cn.Query<Page>(sql, new { ids = ids });
            }
        }
    }
}
