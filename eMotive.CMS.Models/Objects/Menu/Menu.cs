using System.Collections.Generic;

namespace eMotive.CMS.Models.Objects.Menu
{
    public class Menu
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
    }


}
