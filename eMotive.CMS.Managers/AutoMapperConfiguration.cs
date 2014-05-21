using AutoMapper;
using eMotive.CMS.Models.Objects.Application;
using eMotive.CMS.Models.Objects.Search;
using Rep = eMotive.CMS.Repositories.Objects;
using Mod = eMotive.CMS.Models.Objects;
using emSearch = eMotive.CMS.Search.Objects.Search;


namespace eMotive.CMS.Managers
{
    class AutoMapperConfiguration
    {
        public enum Maps { Application, User, Course, Role, Page }

        public static void Configure(params Maps[] toMap)
        {
            foreach (var map in toMap ?? new Maps[0])
            {
                switch (map)
                {
                    case Maps.Application:
                        ConfigureApplicationMapping();
                        break;
                    case Maps.User:
                        ConfigureUserMapping();
                        break;
                    case Maps.Role:
                        ConfigureRoleMapping();
                        break;
                    case Maps.Course:
                        ConfigureCourseMapping();
                        break;
                    case Maps.Page:
                        ConfigurePageMapping();
                        break;
                }
            }

            ConfigureSearchMapping();
        }

        private static void ConfigureApplicationMapping()
        {
            Mapper.CreateMap<Rep.Application.Application, Application>();
            Mapper.CreateMap<Application, Rep.Application.Application>();
            Mapper.CreateMap<Rep.Application.ApplicationRole, ApplicationRole>();
            Mapper.CreateMap<ApplicationRole, Rep.Application.ApplicationRole>();

            Mapper.CreateMap<Rep.Application.CourseYears, CourseYears>();
            Mapper.CreateMap<CourseYears, Rep.Application.CourseYears>();
        }

        private static void ConfigureUserMapping()
        {
          //  Mapper.CreateMap<Rep.User.User, User>();
          //  Mapper.CreateMap<User, Rep.User.User>();
        }

        private static void ConfigureCourseMapping()
        {
            Mapper.CreateMap<Rep.Courses.Course, Mod.Courses.Course>();
            Mapper.CreateMap<Mod.Courses.Course, Rep.Courses.Course>();

            Mapper.CreateMap<Rep.Courses.CourseYear, Mod.Courses.CourseYear>();
            Mapper.CreateMap<Mod.Courses.CourseYear, Rep.Courses.CourseYear>();
        }

        private static void ConfigurePageMapping()
        {
            Mapper.CreateMap<Rep.Pages.Section, Mod.Pages.Section>();
            Mapper.CreateMap<Mod.Pages.Section, Rep.Pages.Section>();

            Mapper.CreateMap<Rep.Pages.Page, Mod.Pages.Page>();
            Mapper.CreateMap<Mod.Pages.Page, Rep.Pages.Page>();

            Mapper.CreateMap<Rep.Pages.PageProjection, Mod.Pages.PageProjection>();
            Mapper.CreateMap<Mod.Pages.PageProjection, Rep.Pages.PageProjection>();
        }

        private static void ConfigureRoleMapping()
        {
            Mapper.CreateMap<Rep.Users.Role, Mod.Roles.Role>();
            Mapper.CreateMap<Mod.Roles.Role, Rep.Users.Role>();
            //  Mapper.CreateMap<User, Rep.User.User>();
        }

        private static void ConfigureSearchMapping()
        {
            Mapper.CreateMap<BasicSearch, emSearch>().ForMember(m => m.CurrentPage, o => o.MapFrom(m => m.Page));
        }
    }
}
