using System.Collections.Generic;
using eMotive.CMS.Repositories.Objects.Users;

namespace eMotive.CMS.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User New();
        User Fetch(int id);
        User Fetch(string value, FetchByUserField field);

        string FetUserNotes(string username);
        bool SaveUserNotes(string username, string notes);

        IEnumerable<User> FetchAll();
        IEnumerable<User> Fetch(IEnumerable<int> ids);
        IEnumerable<User> Fetch(IEnumerable<string> usernames);

        bool Create(User user, out int id);
        bool Update(User user);
        bool Delete(User user);

        bool ValidateUser(string username, string password);

        bool SavePassword(int id, string salt, string password);
        string GetSalt(string username);
    }
}
