using System;

namespace eMotive.CMS.Repositories.Objects.Courses
{
    public class CourseYear
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        //public string BannerCode { get; set; }
        public DateTime YearStart { get; set; }
        public int CourseID { get; set; }
        public int Year { get; set; }

    }
}
