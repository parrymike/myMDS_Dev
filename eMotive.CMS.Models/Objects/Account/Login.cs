using eMotive.CMS.Models.Validation.Account;
using FluentValidation.Attributes;

namespace eMotive.CMS.Models.Objects.Account
{
    [Validator(typeof(LoginValidator))]
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
