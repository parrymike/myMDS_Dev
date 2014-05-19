using System.Collections.Generic;

namespace eMotive.CMS.Repositories.Objects.Application
{
    public class Application
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public IEnumerable<ApplicationRole> Roles { get; set; }
        public IEnumerable<CourseYears> CourseAccess { get; set; }
    }
}
