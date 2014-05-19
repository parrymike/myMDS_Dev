using System.Collections.Generic;

namespace eMotive.CMS.Models.Objects.Menu
{
    public class MenuItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
