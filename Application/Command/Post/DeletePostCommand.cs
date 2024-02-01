using Application.Boudary.Post;
using Application.Command.Post.Validation;
using Infrastructure.Message;
using System.ComponentModel.DataAnnotations;

namespace Application.Command.Post
{
    public class DeletePostCommand : Command<DeletePostOutput>
    {
        public DeletePostCommand() { }

        public DeletePostInput Input { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new DeletePostCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
