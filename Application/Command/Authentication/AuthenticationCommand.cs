using Application.Boudary.User;
using Application.Command.Authentication.Validation;
using Infrastructure.Message;
using System.ComponentModel.DataAnnotations;

namespace Application.Command.Authentication
{
    public class AuthenticationCommand : Command<AuthenticationOutput>
    {
        public AuthenticationCommand() { }

        public AuthenticationInput Input { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new AuthenticateCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
