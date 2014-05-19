using System.Collections.Generic;
using eMotive.CMS.Repositories.Objects.Application;

namespace eMotive.CMS.Repositories.Interfaces
{
    public interface IApplicationRepository
    {
        Application New();

        bool Create(Application application, out int id);

        bool Put(Application application);

        bool Update(Application application);

        bool Delete(Application application);

        Application Fetch(int id);

        Application Fetch(string name);

        IEnumerable<Application> Fetch();

        IEnumerable<Application> Fetch(IEnumerable<int> ids);

    }
}
