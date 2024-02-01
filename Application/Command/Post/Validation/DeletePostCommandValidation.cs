using FluentValidation;

namespace Application.Command.Post.Validation
{
    public class DeletePostCommandValidation : AbstractValidator<DeletePostCommand>
    {
        public DeletePostCommandValidation()
        {
            RuleFor(d => d.Input.Id)
                .NotEmpty().WithMessage("Dados incorretos")
                .GreaterThan(0).WithMessage("Dados incorretos");
        }
    }
}
