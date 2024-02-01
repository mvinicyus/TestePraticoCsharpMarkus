using FluentValidation;

namespace Application.Command.Post.Validation
{
    public class GetPostsCommandValidation : AbstractValidator<GetPostsCommand>
    {
        public GetPostsCommandValidation()
        {
            RuleFor(d => d.Input.Draw)
                .NotEmpty().WithMessage("Dados de paginação devem ser informados")
                .GreaterThan(0).WithMessage("Dados de paginação devem ser positivos");
        }
    }
}
