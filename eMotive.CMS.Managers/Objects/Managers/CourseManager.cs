using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Courses;
using eMotive.CMS.Models.Objects.Search;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Search.Objects;
using eMotive.CMS.Services.Events.CourseManager;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects;
using eMotive.CMS.Services.Objects.Audit;
using Lucene.Net.Search;
using Rep = eMotive.CMS.Repositories.Objects;
using Mod = eMotive.CMS.Models;
using Map = eMotive.CMS.Managers.AutoMapperConfiguration.Maps;
using emSearch = eMotive.CMS.Search.Objects.Search;

namespace eMotive.CMS.Managers.Objects.Managers
{
    public class CourseManager : ICourseManager
    {
        private readonly ICourseRepository _courseRepository;

        public CourseManager(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
            AutoMapperConfiguration.Configure(Map.Course);
        }

        public IMappingEngine Mapper { get; set; }
        public IEventManagerService EventManagerService { get; set; }
        public ISearchManager SearchManager { get; set; }
        public IMessageBusService MessageBusService { get; set; }
        public IAuditService AuditService { get; set; }

        public Course New()
        {
            var app = _courseRepository.New();

            return Mapper.Map<Rep.Courses.Course, Course>(app);
        }

        public bool Create(Course course, out int id)
        {
            id = -1;

            var checkCourse = _courseRepository.Fetch(course.Name);

            if (checkCourse != null)
            {

                /* if (checkApp.Archived)
                 {
                     NotificationService.AddIssue(ResourceManager.ApplicationManager_Create_ApplicationArchived());
                     return false;
                 }*/

                if (String.Equals(course.Name, checkCourse.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    //   MessageBusService.AddIssue(ResourceManager.CourseManager_Create_CourseNameExists());
                    return false;
                }

                if (String.Equals(course.Abbreviation, checkCourse.Abbreviation, StringComparison.InvariantCultureIgnoreCase))
                {
                    //   MessageBusService.AddIssue(ResourceManager.CourseManager_Create_CourseAbbreviationExists());
                    return false;
                }
                /*
                if (course.CourseYears.HasContent())
                {
                    if (course.CourseYears.GroupBy(n => n).Any(m => m.Key.Name.Count() > 1))
                    {
                        //TODO: a duplicate error stating a course year already exists with same name?
                    }
                }*/
            }

            var repCourse = Mapper.Map<Course, Rep.Courses.Course>(course);
            if (_courseRepository.Create(repCourse, out id))
            {
                var newCourse = Fetch(id);
                AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, newCourse);

                EventManagerService.QueueEvent(new CourseCreatedEvent(course));

                return true;
            }

            //   MessageBusService.AddError(ResourceManager.CourseManager_Create_Error());
            return false;
        }

        public bool Update(Course course)
        {
            var checkCourse = _courseRepository.Fetch(course.Name);

            if (checkCourse != null)
            {
                if (String.Equals(course.Name, checkCourse.Name, StringComparison.InvariantCultureIgnoreCase) && course.ID != checkCourse.ID)
                {
                    //  MessageBusService.AddIssue(ResourceManager.CourseManager_Update_CourseNameExists());
                    return false;
                }

                if (String.Equals(course.Abbreviation, checkCourse.Abbreviation, StringComparison.InvariantCultureIgnoreCase) && course.ID != checkCourse.ID)
                {
                    //  MessageBusService.AddIssue(ResourceManager.CourseManager_Update_CourseAbbreviationExists());
                    return false;
                }
            }

            var repApp = Mapper.Map<Course, Rep.Courses.Course>(course);
            if (_courseRepository.Update(repApp))
            {
                var updated = Fetch(course.ID);
                AuditService.ObjectAuditLog(ActionType.Update, n => n.ID, updated);
                EventManagerService.QueueEvent(new CourseUpdatedEvent(updated));

                return true;
            }

            // MessageBusService.AddError(ResourceManager.CourseManager_Update_Error());
            return false;
        }

        public bool Delete(int id)
        {
            //var repApp = Mapper.Map<Course, Rep.Courses.Course>(course);
            var course = Fetch(id);
            if (_courseRepository.Delete(Mapper.Map<Course, Rep.Courses.Course>(course)))
            {
                AuditService.ObjectAuditLog(ActionType.Delete, n => n.ID, course);

                EventManagerService.QueueEvent(new CourseDeletedEvent(course));
                return true;
            }

            // MessageBusService.AddError(ResourceManager.ApplicationManager_Delete_Error());
            return false;
        }

        public IEnumerable<Course> Fetch()
        {
            var courses = _courseRepository.Fetch();

            //if (courses.IsEmpty())
            //   MessageBusService.AddIssue(ResourceManager.CourseManager_Fetch_FetchError());

            return Mapper.Map<IEnumerable<Rep.Courses.Course>, IEnumerable<Course>>(courses);
        }

        public IEnumerable<Course> Fetch(IEnumerable<int> ids)
        {
            var courses = _courseRepository.Fetch(ids);

            //if (courses.IsEmpty())
            //   MessageBusService.AddIssue(ResourceManager.CourseManager_Fetch_FetchError());

            return Mapper.Map<IEnumerable<Rep.Courses.Course>, IEnumerable<Course>>(courses);
        }

        public Course Fetch(int id)
        {
            var app = _courseRepository.Fetch(id);

            return Mapper.Map<Rep.Courses.Course, Course>(app);
        }

        public Course Fetch(string name)
        {
            var app = _courseRepository.Fetch(name);

            return Mapper.Map<Rep.Courses.Course, Course>(app);
        }

        public SearchResult DoSearch(BasicSearch search)
        {
            var newSearch = AutoMapper.Mapper.Map<BasicSearch, emSearch>(search);
            if (string.IsNullOrEmpty(search.Query))
            {
                newSearch.CustomQuery = new Dictionary<string, emSearch.SearchTerm>
                {
                    {"Type", new emSearch.SearchTerm {Field = "Course", Term = Occur.SHOULD}}
                };
            }
            else
            {
                newSearch.CustomQuery = new Dictionary<string, emSearch.SearchTerm>
                {
                    {"Name", new emSearch.SearchTerm {Field = search.Query, Term = Occur.SHOULD}}
                };
            }
            return SearchManager.DoSearch(newSearch);
        }

        public void ReindexSearchRecords()
        {
            /* var records = _courseRepository.Fetch();

             if (!records.HasContent())
             {
                 //todo: send an error message here
                 return;
             }

             foreach (var item in records)
             {
                 SearchManager.Add(new RoleSearchDocument(item));
             }*/
        }

        public IEnumerable<Course> FetchRecordsFromSearch(SearchResult searchResult)
        {
            if (!searchResult.Items.IsEmpty())
            {
                var repItems = _courseRepository.Fetch(searchResult.Items.Select(n => n.ID).ToList());
                if (!repItems.IsEmpty())
                {
                    return AutoMapper.Mapper.Map<IEnumerable<Rep.Courses.Course>, IEnumerable<Course>>(repItems);

                }
            }

            return null;
        }

        public bool RollBack(AuditRecord record)
        {
            var rollBackCourse = record.Object.FromJson<Course>();
            var repApp = Mapper.Map<Course, Rep.Courses.Course>(rollBackCourse);
            var success = _courseRepository.Put(repApp);

            if (success)
            {
                AuditService.ObjectAuditLog(ActionType.RollBack, n => n.ID, rollBackCourse, record);
                EventManagerService.QueueEvent(new CourseRolledBackEvent(rollBackCourse));
            }

            return success;
        }
    }
}
