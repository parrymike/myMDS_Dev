using System.Collections.Generic;

namespace eMotive.CMS.Models.Objects.Courses
{
    public class Course
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string BannerCode { get; set; }

        public IEnumerable<CourseYear> CourseYears { get; set; }
    }
}
