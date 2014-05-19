using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using Dapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Repositories.Objects.Courses;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Repositories.Objects.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public CourseRepository(string connectionString)
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

        public Course New()
        {
            return new Course();
        }

        public bool Create(Course course, out int id)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();

                    var success = true;
                    id = -1;

                    var sql = "INSERT INTO Courses (Name, Abbreviation, BannerCode) Values (@Name, @Abbreviation, @BannerCode);";

                    success &= cn.Execute(sql, course) > 0;

                    var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                    var convId = id = Convert.ToInt32(newId);


                    if (id > 0)
                    {
                        if (course.CourseYears.HasContent())
                        {

                            var courseYears = course.CourseYears.Select(n =>
                            {
                                n.CourseID = convId;
                                return n;
                            }).ToList();

                            sql = "INSERT INTO CourseYears (Name, Abbreviation, BannerCode, YearStart, CourseID, Year) Values (@Name, @Abbreviation, @BannerCode, @YearStart, @CourseID, @Year);";

                            success = cn.Execute(sql, courseYears/*course.CourseYears*/) > 0; //TODO: check this out! should I be passing course.Courseyears in or courseYears list which I attached the newly inserted courseyear ID to each course year.
                            // id = newId;
                        }
                    }

                    transaction.Complete();

                    return success | newId > 0;
                }
            }
        }

        public bool Put(Course course)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();

                    var success = true;


                    var sql =
                        "UPDATE `Courses` SET `Name` = @Name, `Abbreviation` = @Abbreviation, `BannerCode` = @BannerCode WHERE `id`=@id;";

                    success &= cn.Execute(sql, new { Name = course.Name, Abbreviation = course.Abbreviation, BannerCode = course.BannerCode, id = course.ID }) > 0;

                    sql = "DELETE FROM `CourseYears` WHERE `CourseID`=@id;";

                    cn.Execute(sql, new {id = course.ID}); //doesn't work checking rows affected here incase there were no rows in the old item, if there were none, the bool would flip success to false

                    if (course.CourseYears.HasContent())
                    {

                        sql =
                            "INSERT INTO CourseYears (ID, Name, Abbreviation, BannerCode, YearStart, Year, CourseID) Values (@ID, @Name, @Abbreviation, @BannerCode, @YearStart, @Year, @CourseID);";

                        success &= cn.Execute(sql, course.CourseYears) > 0;
                        // id = newId;
                    }

                    if (!success)
                        transaction.Dispose();
                    else
                        transaction.Complete();



                    return success;

                }
            }
        }

        public bool Update(Course course)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();
                    bool success = true;
                    var sql = "SELECT `ID`, `Name`, `Abbreviation`, `BannerCode` FROM `Courses` WHERE `id`=@id;";

                    var oldCourse = cn.Query<Course>(sql, new { id = course.ID }).SingleOrDefault();

                    if (oldCourse != null)
                    {
                        sql = "SELECT `ID`, `Name`, `Abbreviation`, `BannerCode`, `YearStart`, `CourseID`, `Year` FROM `CourseYears` WHERE `CourseID` = @id;";
                        oldCourse.CourseYears = cn.Query<CourseYear>(sql, new { id = course.ID });
                    }

                    sql = "UPDATE `Courses` SET `Name` = @Name, `Abbreviation` = @Abbreviation, `BannerCode` = @BannerCode WHERE `id`=@id;";

                    success &= cn.Execute(sql, new { Name = course.Name, Abbreviation = course.Abbreviation, BannerCode = course.BannerCode, id = course.ID }) > 0;

                    if (course.CourseYears.IsEmpty() && !oldCourse.CourseYears.IsEmpty())
                    {
                        sql = "DELETE FROM `CourseYears` WHERE `CourseID`=@id;";

                        success &= cn.Execute(sql, new { id = course.ID }) > 0;
                    }
                    else
                    {
                        if (oldCourse.CourseYears.IsEmpty() && !course.CourseYears.IsEmpty())
                        {
                            sql =
                                "INSERT INTO `CourseYears` (`Name`, `Abbreviation`, `BannerCode`, `YearStart`, `CourseID`, `Year`) VALUES (@Name, @Abbreviation, @BannerCode, @YearStart, @CourseID, @Year);";
                            var toCreateAll = course.CourseYears.Select(n =>
                            {
                                n.CourseID = course.ID;
                                return n;
                            }).ToList();

                            success &= cn.Execute(sql, toCreateAll) > 0;
                        }
                        else if (!oldCourse.CourseYears.IsEmpty() && !course.CourseYears.IsEmpty())
                        {
                            //todo: check for null?
                            var toDelete =
                                oldCourse.CourseYears.Where(n => !course.CourseYears.Any(m => n.ID == m.ID && m.ID > 0));

                            var toUpdate = course.CourseYears.Where(n => oldCourse.CourseYears.Any(m => n.ID == m.ID));

                            var toCreate = course.CourseYears.Where(n => n.ID == 0);

                            var toCreateFinal = toCreate.Select(n =>
                            {
                                n.CourseID = course.ID;
                                return n;
                            }).ToList();

                            if (!toDelete.IsEmpty())
                            {
                                sql = "DELETE FROM `CourseYears` WHERE `id`=@id;";

                                success &= cn.Execute(sql, toDelete) > 0;
                            }

                            if (!toUpdate.IsEmpty())
                            {
                                sql =
                                    "UPDATE `CourseYears` SET `Name` = @Name, `Abbreviation` = @Abbreviation, `BannerCode` = @BannerCode, `YearStart` = @YearStart, `Year` = @Year WHERE `id`=@id;";
                                success &= cn.Execute(sql, toUpdate) > 0;
                            }

                            if (!toCreate.IsEmpty())
                            {
                                sql =
                                    "INSERT INTO `CourseYears` (`Name`, `Abbreviation`, `BannerCode`, `YearStart`, `CourseID`, `Year`) VALUES (@Name, @Abbreviation, @BannerCode, @YearStart, @CourseID, @Year );";
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

        public bool Delete(Course course)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();
                    var success = true;

                    var sql = "DELETE FROM `Courses` WHERE `Id`=@id;";
                    success &= cn.Execute(sql, new { id = course.ID }) > 0;

                    sql = "DELETE FROM `CourseYears` WHERE `CourseID`=@CourseID;";

                    success &= cn.Execute(sql, new { CourseID = course.ID }) > 0;

                    transaction.Complete();

                    return success;
                }

            }
        }

        public Course Fetch(int id)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT ID, Name, Abbreviation, BannerCode FROM Courses WHERE id=@id;";

                var course = cn.Query<Course>(sql, new { id = id }).SingleOrDefault();

                if (course != null)
                {
                    sql = "SELECT ID, Name, Abbreviation, BannerCode, YearStart, CourseID, Year FROM CourseYears WHERE CourseID = @id;";
                    course.CourseYears = cn.Query<CourseYear>(sql, new { id = course.ID });
                }

                return course;
            }
        }

        public Course Fetch(string name)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT ID, Name, Abbreviation, BannerCode FROM Courses WHERE name=@name;";

                var course = cn.Query<Course>(sql, new { name = name }).SingleOrDefault();

                if (course != null)
                {
                    sql = "SELECT ID, Name, Abbreviation, BannerCode, YearStart, CourseID, Year FROM CourseYears WHERE CourseID = @id;";
                    course.CourseYears = cn.Query<CourseYear>(sql, new { id = course.ID });
                }

                return course;
            }
        }

        public IEnumerable<Course> Fetch()
        {
            using (var cn = Connection)
            {
                var sql = "SELECT ID, Name, Abbreviation, BannerCode FROM Courses;";

                var courses = cn.Query<Course>(sql);

                if (!courses.IsEmpty())
                {
                    sql = "SELECT ID, Name, Abbreviation, BannerCode, YearStart, CourseID, Year FROM CourseYears;";
                    var courseYears = cn.Query<CourseYear>(sql);

                    if (!courseYears.IsEmpty())
                    {
                        var courseYearDict = courseYears.GroupBy(m => m.CourseID).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var course in courses)
                        {
                            List<CourseYear> courseYearsList;
                            if (courseYearDict.TryGetValue(course.ID, out courseYearsList))
                                course.CourseYears = courseYearsList;
                        }
                    }
                }

                return courses;
            }
        }

        public IEnumerable<Course> Fetch(IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT ID, Name, Abbreviation, BannerCode FROM Courses WHERE id=@ids;";//todo: using = here rather than IN - see what format the sql statement takes.

                var courses = cn.Query<Course>(sql, new { ids = ids });

                if (!courses.IsEmpty())
                {
                    sql = "SELECT ID, Name, Abbreviation, BannerCode, YearStart, CourseID, Year FROM CourseYears WHERE CourseID IN @ids;";

                    var results = cn.Query<CourseYear>(sql, new { ids = ids });

                    if (!results.IsEmpty())
                    {
                        var resultDict = results.GroupBy(n => n.CourseID).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var course in courses)
                        {
                            List<CourseYear> yearsList;
                            if (resultDict.TryGetValue(course.ID, out yearsList))
                            {
                                course.CourseYears = yearsList;
                            }
                        }
                    }
                }

                return courses;
            }

        }
    }
}
