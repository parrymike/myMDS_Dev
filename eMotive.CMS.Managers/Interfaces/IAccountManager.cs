using eMotive.CMS.Models.Objects.Account;
using eMotive.CMS.Models.Objects.Users;

namespace eMotive.CMS.Managers.Interfaces
{
    public interface IAccountManager
    {
        bool ValidateUser(string username, string password);
        bool CreateNewAccountPassword(User user);
        bool ResendPassword(string email);
        bool ResendUsername(string email);
        bool ChangePassword(ChangePassword changePassword);
        bool ResendAccountCreationEmail(string username);
    }
}
