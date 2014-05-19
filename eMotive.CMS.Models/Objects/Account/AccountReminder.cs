using eMotive.CMS.Models.Validation.Account;
using FluentValidation.Attributes;

namespace eMotive.CMS.Models.Objects.Account
{
    [Validator(typeof(AccountReminderValidator))]
    public class AccountReminder
    {
        public string EmailAddress { get; set; }
    }
}
