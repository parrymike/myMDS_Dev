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
                                        URL = "/Test/Course",
                                        Icon = "menu-icon fa fa-folder-o",
                                        Title = "Manage Courses"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Application Admin",
                                        URL = "/Test/Application",
                                        Icon = "menu-icon fa fa-cog",
                                        Title = "Manage Applications"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Email Admin",
                                        URL = "/Test/Emails",
                                        Icon = "menu-icon fa fa-bolt",
                                        Title = "Manage Email Events"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "PageAdmin",
                                        URL = "/Test/Pages",
                                        Icon = "menu-icon fa fa-book",
                                        Title = "Manage Pages"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "User Admin",
                                        URL = "/Test/Users",
                                        Icon = "menu-icon fa fa-users",
                                        Title = "Manage Users"
                                    }
                                /*
                                new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Course Admin",
                                        URL = "/portals/intrameddevtest/Test/Course",
                                        Icon = "menu-icon fa fa-folder-o",
                                        Title = "Manage Courses"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Application Admin",
                                        URL = "/portals/intrameddevtest/Test/Application",
                                        Icon = "menu-icon fa fa-cog",
                                        Title = "Manage Applications"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Email Admin",
                                        URL = "/portals/intrameddevtest/Test/Emails",
                                        Icon = "menu-icon fa fa-bolt",
                                        Title = "Manage Email Events"
                                    }
                                 */
                            }
            };

            return menu;
        }
    }
}
