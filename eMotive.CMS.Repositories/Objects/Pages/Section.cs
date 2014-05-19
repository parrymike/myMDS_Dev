using System.Collections.Generic;

namespace eMotive.CMS.Repositories.Objects.Pages
{
    public class Section
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<PageProjection> Pages { get; set; } 
    }
}
