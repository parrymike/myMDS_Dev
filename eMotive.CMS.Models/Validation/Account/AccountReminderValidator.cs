using eMotive.CMS.Models.Objects.Account;
using FluentValidation;

namespace eMotive.CMS.Models.Validation.Account
{
    public class AccountReminderValidator : AbstractValidator<AccountReminder>
    {
        public AccountReminderValidator()
        {
            RuleFor(n => n.EmailAddress).NotEmpty().WithMessage("Please enter your email address.").EmailAddress().WithMessage("Please enter a valid email address.");
        }
    }
}
