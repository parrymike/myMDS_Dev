using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Roles;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;

namespace eMotive.CMS.Managers.Interfaces
{
    public interface IRoleManager : ISearchable<Role>, IAuditable
    {
        Role New();
        Role Fetch(int id);
        Role Fetch(string name);

        bool Create(Role role, out int id);
        bool Update(Role role);
        bool Delete(Role role);
        IEnumerable<Role> FetchAll();
        IEnumerable<Role> Fetch(IEnumerable<int> ids);

        bool RollBack(AuditRecord record);
    }
}
