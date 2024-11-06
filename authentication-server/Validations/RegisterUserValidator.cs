using authentication_server.Common.Constants;
using authentication_server.DTOs;
using FluentValidation;

namespace authentication_server.Validations
{
    public class RegisterUserValidator : AbstractValidator<UserRegistrationDTO>
    {
        public RegisterUserValidator() 
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Matches(Regexp.PasswordStrength);
            RuleFor(x => x.PasswordConfirm).NotEmpty().Matches(Regexp.PasswordStrength);
        }
    }
}
