using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace eMotive.CMS.Models.Objects.Application
{
    public class Application
    {
        public Application()
        {
            Roles = new Collection<ApplicationRole>();
            CourseAccess = new Collection<CourseYears>();
        }

        public int ID { get; set; }
        public string Name { get; set; }

        public IEnumerable<ApplicationRole> Roles { get; set; }
        public IEnumerable<CourseYears> CourseAccess { get; set; }
    }
}
