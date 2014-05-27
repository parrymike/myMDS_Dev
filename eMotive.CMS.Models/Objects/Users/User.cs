using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Roles;

namespace eMotive.CMS.Models.Objects.Users
{
    public class User
    {
        /*
         * public int ID { get; set; }
        public string Username { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public bool Archived { get; set; }

        public IEnumerable<Role> Roles { get; set; }

        public string RoleString { get; set; } //TODO: do we really need this? Perhaps angularjs user creation?
        */

        public int ID { get; set; }
        public string Username { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string UniversityID { get; set; }
        public string RegistrationStatus { get; set; }
        public bool Enabled { get; set; }
        public bool Archived { get; set; }
        public System.DateTime LastLogin { get; set; }

        public UserType UserType { get; set; }

        public IEnumerable<Role> Roles { get; set; }
    }
}
