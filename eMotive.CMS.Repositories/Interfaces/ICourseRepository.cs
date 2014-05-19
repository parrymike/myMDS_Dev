using System.Collections.Generic;
using eMotive.CMS.Repositories.Objects.Courses;

namespace eMotive.CMS.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        /// <summary>
        /// Returns a new Course object
        /// </summary>
        /// <returns>An empty Course class</returns>
        Course New();

        /// <summary>
        /// Creates a new Course object and returns it's newly created DB id
        /// </summary>
        /// <param name="course">The Course object to insert in the database</param>
        /// <param name="id">The id of the newly created object</param>
        /// <returns>Bool indicating if creation was a success</returns>
        bool Create(Course course, out int id);

        /// <summary>
        /// Replaces a course object
        /// </summary>
        /// <param name="course">The Course object to be repaced</param>
        /// <returns>Bool indicating if the replace was a success</returns>
        bool Put(Course course);


        /// <summary>
        /// Updates a Course object
        /// </summary>
        /// <param name="course">The Course object to update</param>
        /// <returns>Bool indicating if the update was a success</returns>
        bool Update(Course course);

        /// <summary>
        /// Deletes a Course object
        /// </summary>
        /// <param name="course">The Course object to delete</param>
        /// <returns>Bool indicating if te deletion was a success</returns>
        bool Delete(Course course);

        /// <summary>
        /// Fetches an Course object
        /// </summary>
        /// <param name="id">Id of the Course to fetch</param>
        /// <returns>The requested Course object</returns>
        Course Fetch(int id);

        /// <summary>
        /// Fetches an Course object
        /// </summary>
        /// <param name="name">Name of the Course to fetch</param>
        /// <returns>The requested Course object</returns>
        Course Fetch(string name);

        /// <summary>
        /// Fetches all Course objects
        /// </summary>
        /// <returns>A collection containing all Course objects</returns>
        IEnumerable<Course> Fetch();

        IEnumerable<Course> Fetch(IEnumerable<int> ids);
    }
}
