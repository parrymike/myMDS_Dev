using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Courses;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Services.Interfaces;

namespace eMotive.CMS.Managers.Interfaces
{
    public interface ICourseManager : ISearchable<Course>, IAuditable
    {
        Course New();
        bool Create(Course course, out int id);
        bool Update(Course course);
        bool Delete(int id);

        IEnumerable<Course> Fetch();
        IEnumerable<Course> Fetch(IEnumerable<int> ids);
        Course Fetch(int id);
        Course Fetch(string name);
    }
}
