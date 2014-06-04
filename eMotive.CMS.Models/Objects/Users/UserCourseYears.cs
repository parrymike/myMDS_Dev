using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eMotive.CMS.Models.Objects.Courses;

namespace eMotive.CMS.Models.Objects.Users
{
    public class UserCourseYear : CourseYear
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public CourseYear CourseYear { get; set; }
        public string AcademicYear { get; set; }
    }
}
