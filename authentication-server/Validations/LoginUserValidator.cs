using authentication_server.Common.Constants;
using authentication_server.DTOs;
using FluentValidation;

namespace authentication_server.Validations
{
    public class LoginUserValidator : AbstractValidator<UserLoginDTO>
    {
        public LoginUserValidator() 
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Matches(Regexp.PasswordStrength);
        }
    }
}
