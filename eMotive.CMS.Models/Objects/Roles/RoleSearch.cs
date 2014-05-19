using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Search;

namespace eMotive.CMS.Models.Objects.Roles
{
    public class RoleSearch : BasicSearch
    {
        public RoleSearch()
        {
            ItemType = "Roles";
            Type = new[] { "Role" }; ;
        }

        public IEnumerable<Role> Roles { get; set; }
    }
}
