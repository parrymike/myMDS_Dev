﻿using eMotive.CMS.Managers.Interfaces;
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
                                        Title = "Course Test page"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Application Admin",
                                        URL = "/Test/Application",//"/SCE/Account/Login",
                                        Title = "Application Test page"
                                    },
                                    new MenuItem
                                    {
                                        ID = 1,
                                        Name = "Email Admin",
                                        URL = "/Test/Emails",//"/SCE/Account/Login",
                                        Title = "Email Test page"
                                    }
                            }
            };

            return menu;
        }
    }
}