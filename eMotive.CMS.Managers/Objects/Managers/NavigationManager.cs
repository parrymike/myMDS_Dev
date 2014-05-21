using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Menu;
namespace eMotive.CMS.Managers.Objects.Managers
{
    public class NavigationManager : INavigationManager
    {
        public Menu GetTestMenu()
        {
            var menu = new Menu
            {
                ID = 1,
                Title = "TestMenu",
                MenuItems = new[]
                            {
                                 new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Course Admin",
                                        URL = "/Test/Course",//"/SCE/Account/Login",/portals/intrameddevtest/Test/Course
                                        Icon = "menu-icon fa fa-folder-o",
                                        Title = "Course Admin"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Application Admin",
                                        URL = "/Test/Application",//"/SCE/Account/Login",
                                        Icon = "menu-icon fa fa-cog",
                                        Title = "Application Admin"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Email Admin",
                                        URL = "/Test/Emails",//"/SCE/Account/Login",
                                        Icon = "menu-icon fa fa-bolt",
                                        Title = "Email Test page"
                                    }
                            }
            };

            return menu;
        }
    }
}
