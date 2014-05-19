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
        public bool Enabled { get; set; }
        public bool Archived { get; set; }

        public IEnumerable<Role> Roles { get; set; }
    }
}
