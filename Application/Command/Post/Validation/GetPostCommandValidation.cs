using FluentValidation;

namespace Application.Command.Post.Validation
{
    public class GetPostCommandValidation : AbstractValidator<GetPostCommand>
    {
        public GetPostCommandValidation()
        {
            RuleFor(d => d.Input.Id)
                .NotEmpty().WithMessage("Dados incorretos")
                .GreaterThan(0).WithMessage("Dados incorretos");
        }
    }
}
