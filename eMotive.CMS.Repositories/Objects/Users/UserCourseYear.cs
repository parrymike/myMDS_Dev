using eMotive.CMS.Repositories.Objects.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotive.CMS.Repositories.Objects.Users
{
    public class UserCourseYear : CourseYear
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public CourseYear CourseYear { get; set; }
        public string AcademicYear { get; set; }
        public bool CurrentYear { get; set; }
    }
}
