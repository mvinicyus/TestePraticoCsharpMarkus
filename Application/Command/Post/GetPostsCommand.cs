using Application.Boudary.Post;
using Application.Command.Post.Validation;
using Infrastructure.Message;
using System.ComponentModel.DataAnnotations;

namespace Application.Command.Post
{
    public class GetPostsCommand : Command<GetPostsOutput>
    {
        public GetPostsCommand() { }

        public GetPostsInput Input { get; set; }
        public override bool IsValid()
        {
            ValidationResult = new GetPostsCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
