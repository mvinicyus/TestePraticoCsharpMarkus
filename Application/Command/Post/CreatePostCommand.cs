using Application.Boudary.Post;
using Application.Command.Post.Validation;
using Infrastructure.Message;
using System.ComponentModel.DataAnnotations;

namespace Application.Command.Post
{
    public class CreatePostCommand : Command<CreatePostOutput>
    {
        public CreatePostCommand() { }

        public CreatePostInput Input { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new CreatePostCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
