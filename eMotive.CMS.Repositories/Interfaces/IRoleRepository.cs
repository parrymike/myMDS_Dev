using System.Collections.Generic;
using eMotive.CMS.Repositories.Objects.Users;

namespace eMotive.CMS.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Role New();

        Role Fetch(int id);
        Role Fetch(string name);

        IEnumerable<Role> Fetch(IEnumerable<int> ids);
        IEnumerable<Role> Fetch(IEnumerable<string> names);

        IEnumerable<Role> FetchAll();

        bool AddUserToRoles(int id, IEnumerable<int> ids);
        bool RemoveUserFromRoles(int userId, IEnumerable<int> ids);
        IEnumerable<int> FindUsersInRole(int id);

        IEnumerable<Role> FetchUserRoles(int userId);
        bool Update(Role role);
        bool Create(Role role);
        bool Delete(string role);
        bool Delete(int id);
    }
}
