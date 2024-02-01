using Application.Boudary.Post;
using Application.Command.Post.Validation;
using Infrastructure.Message;
using System.ComponentModel.DataAnnotations;

namespace Application.Command.Post
{
    public class GetPostCommand : Command<GetPostOutput>
    {
        public GetPostCommand() { }

        public GetPostInput Input { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new GetPostCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
