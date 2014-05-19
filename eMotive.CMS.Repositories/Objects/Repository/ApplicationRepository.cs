using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;
using Dapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Repositories.Objects.Application;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Repositories.Objects.Repository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public ApplicationRepository(string connectionString)
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

        public Application.Application New()
        {
            return new Application.Application();
        }

        public bool Create(Application.Application application, out int id)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();
                    var success = true;
                    id = -1;

                    var sql = "INSERT INTO Applications (Name) Values (@Name);";

                    success &= cn.Execute(sql, application) > 0;

                    var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                    var convId = id = Convert.ToInt32(newId);


                    if (id > 0)
                    {
                        var applicationRoles = application.Roles.Select(n =>
                        {
                            n.ApplicationID = convId;
                            return n;
                        }).ToList();

                        sql = "INSERT INTO ApplicationRoles (ApplicationID, Name) Values (@ApplicationID, @Name);";

                        success &= cn.Execute(sql, applicationRoles) > 0;

                        if (!application.CourseAccess.IsEmpty())
                        {
                            var courseAccess =
                                application.CourseAccess.Select(
                                    n =>
                                    {
                                        n.ApplicationID = convId;
                                        return n;
                                    });

                            sql = "INSERT INTO ApplicationCourseYears (ApplicationID, CourseYearID) Values (@ApplicationID, @CourseYearID);";

                            success &= cn.Execute(sql, courseAccess) > 0;
                        }
                    }

                 //   if (!success | newId > 0)
                //        transaction.Dispose();
                 //   else
                        transaction.Complete();

                    return success | newId > 0;
                }
            }
        }

        public bool Put(Application.Application application)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();

                    var success = true;

                    var sql = "UPDATE `Applications` SET `Name`=@Name WHERE `ID`=@ID;";

                    success &= cn.Execute(sql, new { Name = application.Name, ID = application.ID }) > 0;

                    sql = "DELETE FROM `ApplicationRoles` WHERE `ApplicationID`=@id;";

                    /*success &=*/ cn.Execute(sql, new { id = application.ID });// > 0;

                    if (!application.Roles.IsEmpty())
                    {
                        sql =
                            "INSERT INTO `ApplicationRoles` (`ID`, `ApplicationID`, `Name`) Values (@ID, @ApplicationID, @Name);";

                        success &= cn.Execute(sql, application.Roles) > 0;
                    }

                    sql = "DELETE FROM `ApplicationCourseYears` WHERE `ApplicationID`=@id;";

                    /*success &=*/ cn.Execute(sql, new { id = application.ID });// > 0;

                    if (!application.CourseAccess.IsEmpty())
                    {

                        sql = "INSERT INTO `ApplicationCourseYears` (`ID`, `ApplicationID`, `CourseYearID`) Values (@ID, @ApplicationID, @CourseYearID);";

                        success &= cn.Execute(sql, application.CourseAccess) > 0;
                    }


                    if (!success)
                        transaction.Dispose();
                    else
                        transaction.Complete();

                    return success;
                }
            }
        }

        public bool Update(Application.Application application)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();
                    bool success = true;

                    #region fetch old application obj with same ID

                    var sql = "SELECT `ID`, `Name` FROM `Applications` WHERE `ID` = @id;";
                    var oldApplication =
                        cn.Query<Application.Application>(sql, new {id = application.ID}).SingleOrDefault();

                    if (oldApplication != null)
                    {
                        sql =
                            "SELECT `ID`, `ApplicationID`, `CourseYearID` FROM `ApplicationCourseYears` WHERE `ApplicationID` = @id;";
                        oldApplication.CourseAccess = cn.Query<CourseYears>(sql, new {id = application.ID});

                        sql =
                            "SELECT `ID`, `ApplicationID`, `Name` FROM `ApplicationRoles` WHERE `ApplicationID` = @id;";
                        oldApplication.Roles = cn.Query<ApplicationRole>(sql, new {id = application.ID});
                    }

                    #endregion

                    sql = "UPDATE `Applications` SET `Name`=@Name WHERE `ID`=@ID;";
                    success &= cn.Execute(sql, new {Name = application.Name, ID = application.ID}) > 0;

                    if (application.Roles.IsEmpty() && !oldApplication.Roles.IsEmpty())
                    {
                        sql = "DELETE FROM `ApplicationRoles` WHERE `ApplicationID`=@ApplicationID;";
                        success &= cn.Execute(sql, new {ApplicationID = application.ID}) > 0;
                    }
                    else
                    {
                        if (!application.Roles.IsEmpty() && oldApplication.Roles.IsEmpty())
                        {
                            var applicationRoles = application.Roles.Select(n =>
                            {
                                n.ApplicationID = application.ID;
                                return n;
                            }).ToList();

                            sql = "INSERT INTO `ApplicationRoles` (ApplicationID, Name) Values (@ApplicationID, @Name);";

                            success = cn.Execute(sql, applicationRoles) > 0;
                        }
                        else if (!application.Roles.IsEmpty() && !oldApplication.Roles.IsEmpty())
                        {
                            var toDelete =
                                oldApplication.Roles.Where(n => !application.Roles.Any(m => n.ID == m.ID && m.ID > 0));

                            var toUpdate = application.Roles.Where(n => oldApplication.Roles.Any(m => n.ID == m.ID));

                            var toCreate = application.Roles.Where(n => n.ID == 0);

                            var toCreateFinal = toCreate.Select(n =>
                            {
                                n.ApplicationID = application.ID;
                                return n;
                            }).ToList();

                            if (!toDelete.IsEmpty())
                            {
                                sql = "DELETE FROM `ApplicationRoles` WHERE `id`=@id;";

                                success &= cn.Execute(sql, toDelete) > 0;
                            }

                            if (!toUpdate.IsEmpty())
                            {
                                sql =
                                    "UPDATE `ApplicationRoles` SET `Name` = @Name, `ApplicationID` = @ApplicationID WHERE `id`=@id;";
                                success &= cn.Execute(sql, toUpdate) > 0;
                            }

                            if (!toCreate.IsEmpty())
                            {
                                sql =
                                    "INSERT INTO `ApplicationRoles` (`ID`, `ApplicationID`, `Name`) Values (@ID, @ApplicationID, @Name);";
                                success &= cn.Execute(sql, toCreateFinal) > 0;
                                //  _connection.Execute()
                            }
                        }
                    }


                    if (application.CourseAccess.IsEmpty() && !oldApplication.CourseAccess.IsEmpty())
                    {
                        sql = "DELETE FROM `ApplicationCourseYears` WHERE `ApplicationID`=@ApplicationID;";
                        success &= cn.Execute(sql, new { ApplicationID = application.ID }) > 0;
                    }
                    else
                    {
                        if (!application.CourseAccess.IsEmpty() && oldApplication.CourseAccess.IsEmpty())
                        {
                            var applicationCourseYears = application.CourseAccess.Select(n =>
                            {
                                n.ApplicationID = application.ID;
                                return n;
                            }).ToList();

                            sql = "INSERT INTO `ApplicationCourseYears` (ApplicationID, CourseYearID) Values (@ApplicationID, @CourseYearID);";

                            success = cn.Execute(sql, applicationCourseYears) > 0;
                        }
                        else if (!application.CourseAccess.IsEmpty() && !oldApplication.CourseAccess.IsEmpty())
                        {
                            //TODO: we reattach the ID to existing items. This seemed more difficult to do on the angularside.
                            foreach (var oldItem in oldApplication.CourseAccess)
                            {
                                foreach (var newItem in application.CourseAccess)
                                {
                                    if (oldItem.CourseYearID == newItem.CourseYearID)
                                        newItem.ID = oldItem.ID;
                                }
                            }

                            var toDelete = oldApplication.CourseAccess.Where(n => !application.CourseAccess.Any(m => n.ID == m.ID && m.ID > 0));

                            var toUpdate = application.CourseAccess.Where(n => oldApplication.CourseAccess.Any(m => n.ID == m.ID));

                            var toCreate = application.CourseAccess.Where(n => n.ID == 0);

                            var toCreateFinal = toCreate.Select(n =>
                            {
                                n.ApplicationID = application.ID;
                                return n;
                            }).ToList();

                            if (!toDelete.IsEmpty())
                            {
                                sql = "DELETE FROM `ApplicationCourseYears` WHERE `id`=@id;";

                                success &= cn.Execute(sql, toDelete) > 0;
                            }

                            if (!toUpdate.IsEmpty())
                            {
                                sql =
                                    "UPDATE `ApplicationCourseYears` SET `CourseYearID` = @CourseYearID, `ApplicationID` = @ApplicationID WHERE `id`=@id;";
                                success &= cn.Execute(sql, toUpdate) > 0;
                            }

                            if (!toCreate.IsEmpty())
                            {
                                sql =
                                    "INSERT INTO `ApplicationCourseYears` (`ID`, `ApplicationID`, `CourseYearID`) Values (@ID, @ApplicationID, @CourseYearID);";
                                success &= cn.Execute(sql, toCreateFinal) > 0;
                                //  _connection.Execute()
                            }
                        }
                    }


                    if (!success)
                        transaction.Dispose();
                    else
                        transaction.Complete();

                    return success;
                }
            }
        }

        public bool Delete(Application.Application application)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();
                    var success = true;

                    var sql = "DELETE FROM `Applications` WHERE `Id`=@id;";
                    success &= cn.Execute(sql, new { id = application.ID }) > 0;

                    if (!application.CourseAccess.IsEmpty())
                    {
                        sql = "DELETE FROM `ApplicationCourseYears` WHERE `ApplicationID`=@ApplicationID;";

                        success &= cn.Execute(sql, new {ApplicationID = application.ID}) > 0;
                    }

                    if (!application.Roles.IsEmpty())
                    {
                        sql = "DELETE FROM `ApplicationRoles` WHERE `ApplicationID`=@ApplicationID;";

                        success &= cn.Execute(sql, new {ApplicationID = application.ID}) > 0;
                    }

                    if (!success)
                        transaction.Dispose();
                    else
                        transaction.Complete();

                    return success;
                }

            }
        }

        public Application.Application Fetch(int id)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `Applications` WHERE `ID` = @id;";
                var application = cn.Query<Application.Application>(sql, new {id = id}).SingleOrDefault();

                if (application != null)
                {
                    sql = "SELECT `ID`, `ApplicationID`, `CourseYearID` FROM `ApplicationCourseYears` WHERE `ApplicationID` = @id;";
                    application.CourseAccess = cn.Query<CourseYears>(sql, new {id = id});

                    sql = "SELECT `ID`, `ApplicationID`, `Name` FROM `ApplicationRoles` WHERE `ApplicationID` = @id;";
                    application.Roles = cn.Query<ApplicationRole>(sql, new { id = id });
                }

                return application;
            }
        }

        public Application.Application Fetch(string name)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `Applications` WHERE `Name` = @name;";
                var application = cn.Query<Application.Application>(sql, new { name = name }).SingleOrDefault();

                if (application != null)
                {
                    sql = "SELECT `ID`, `ApplicationID`, `CourseYearID` FROM `ApplicationCourseYears` WHERE `ApplicationID` = @id;";
                    application.CourseAccess = cn.Query<CourseYears>(sql, new { id = application.ID });

                    sql = "SELECT `ID`, `ApplicationID`, `Name` FROM `ApplicationRoles` WHERE `ApplicationID` = @id;";
                    application.Roles = cn.Query<ApplicationRole>(sql, new { id = application.ID });
                }

                return application;
            }
        }

        public IEnumerable<Application.Application> Fetch()
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `Applications`;";
                var applications = cn.Query<Application.Application>(sql);

                if (!applications.IsEmpty())
                {
                    sql = "SELECT `ID`, `ApplicationID`, `CourseYearID` FROM `ApplicationCourseYears`;";
                    var courseYears = cn.Query<CourseYears>(sql);//todo: rename courseyears!

                    if (!courseYears.IsEmpty())
                    {
                        var courseyearsDict = courseYears.GroupBy(m => m.ApplicationID).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var app in applications)
                        {
                            List<CourseYears> courseyearsList;

                            if (courseyearsDict.TryGetValue(app.ID, out courseyearsList))
                                app.CourseAccess = courseyearsList;
                        }
                    }

                    sql = "SELECT `ID`, `ApplicationID`, `Name` FROM `ApplicationRoles`;";
                    var appRoles = cn.Query<ApplicationRole>(sql);

                    if (!appRoles.IsEmpty())
                    {
                        var appRolesDict = appRoles.GroupBy(m => m.ApplicationID).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var app in applications)
                        {
                            List<ApplicationRole> appRolesList;

                            if (appRolesDict.TryGetValue(app.ID, out appRolesList))
                                app.Roles = appRolesList;
                        }
                    }
                }

                return applications;
            }
        }

        public IEnumerable<Application.Application> Fetch(IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `ID`, `Name` FROM `Applications` WHERE `ID` IN @ids;";
                var applications = cn.Query<Application.Application>(sql, new {ids = ids});

                if (!applications.IsEmpty())
                {
                    sql = "SELECT `ID`, `ApplicationID`, `CourseYearID` FROM `ApplicationCourseYears` WHERE `ApplicationID` IN @ids;";
                    var courseYears = cn.Query<CourseYears>(sql, new { ids = ids });//todo: rename courseyears!

                    if (!courseYears.IsEmpty())
                    {
                        var courseyearsDict = courseYears.GroupBy(m => m.ApplicationID).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var app in applications)
                        {
                            List<CourseYears> courseyearsList;

                            if (courseyearsDict.TryGetValue(app.ID, out courseyearsList))
                                app.CourseAccess = courseyearsList;
                        }
                    }

                    sql = "SELECT `ID`, `ApplicationID`, `Name` FROM `ApplicationRoles`  WHERE `ApplicationID` IN @ids;";
                    var appRoles = cn.Query<ApplicationRole>(sql, new { ids = ids });

                    if (!appRoles.IsEmpty())
                    {
                        var appRolesDict = appRoles.GroupBy(m => m.ApplicationID).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var app in applications)
                        {
                            List<ApplicationRole> appRolesList;

                            if (appRolesDict.TryGetValue(app.ID, out appRolesList))
                                app.Roles = appRolesList;
                        }
                    }
                }

                return applications;
            }
        }
    }
}
