using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Application;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Services.Interfaces;

namespace eMotive.CMS.Managers.Interfaces
{
    public interface IApplicationManager : ISearchable<Application>, IAuditable
    {
        Application New();
        bool Create(Application application, out int id);
        bool Update(Application application);
        bool Delete(int id);

        IEnumerable<Application> Fetch();
        IEnumerable<Application> Fetch(IEnumerable<int> ids);
        Application Fetch(int id);
        Application Fetch(string name);
    }
}
