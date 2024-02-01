using Application.Boudary.Post;
using Application.Command.Post.Validation;
using Infrastructure.Message;
using System.ComponentModel.DataAnnotations;

namespace Application.Command.Post
{
    public class UpdatePostCommand : Command<UpdatePostOutput>
    {
        public UpdatePostCommand() { }

        public UpdatePostInput Input { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new UpdatePostCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
