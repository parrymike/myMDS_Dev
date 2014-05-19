using eMotive.CMS.Models.Objects.Account;
using FluentValidation;

namespace eMotive.CMS.Models.Validation.Account
{
    public class ChangePasswordValidator : AbstractValidator<ChangePassword>
    {
        public ChangePasswordValidator()
        {
            RuleFor(n => n.CurrentPassword).NotEmpty().WithMessage("Please enter your current password.");

            RuleFor(n => n.NewPassword).NotEmpty().WithMessage("Please enter a new password.")
                                       .Length(5, 20).WithMessage("Your new password must be between 5 and 20 characters long.")
                                       .Equal(n => n.NewPasswordRetype).WithMessage("The new password and retyped password did not match.");

            RuleFor(n => n.NewPasswordRetype).NotEmpty().WithMessage("Please retype your new password.");
        }
    }
}
