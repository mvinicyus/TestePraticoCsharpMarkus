using Application.Boudary.User;
using Application.Command.User.Validation;
using Infrastructure.Message;
using System.ComponentModel.DataAnnotations;

namespace Application.Command.User
{
    public class CreateUserCommand : Command<CreateUserOutput>
    {
        public CreateUserCommand() { }

        public CreateUserInput Input { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new CreateUserCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
