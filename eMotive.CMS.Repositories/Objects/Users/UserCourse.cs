using eMotive.CMS.Repositories.Objects.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotive.CMS.Repositories.Objects.Users
{
    public class UserCourse : Course
    {
        public int ID { get; set; }
        private int UserID { get; set; }
        public string AcademicYear { get; set; }
    }
}
