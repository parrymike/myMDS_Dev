using System;
using System.Collections.Generic;

namespace eMotive.CMS.Repositories.Objects.Users
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string UniversityID { get; set; }
        public string RegistrationStatus { get; set; }
        public bool Enabled { get; set; }
        public bool Archived { get; set; }
        public DateTime LastLogin { get; set; }

        public UserType UserType { get; set; }


        public IEnumerable<UserCourseYear> UserCourseYears { get; set; }
        public IEnumerable<Role> Roles { get; set; }

    }
}
