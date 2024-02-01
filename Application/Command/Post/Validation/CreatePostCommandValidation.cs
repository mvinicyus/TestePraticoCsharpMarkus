using FluentValidation;

namespace Application.Command.Post.Validation
{
    public class CreatePostCommandValidation : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidation()
        {
            RuleFor(d => d.Input.Title)
                .NotEmpty().WithMessage("Preencha o Título")
                .MaximumLength(50).WithMessage("Título deve ter no máximo 50 caracteres")
                .MinimumLength(10).WithMessage("Título deve ter no mínimo 10 caracteres");

            RuleFor(d => d.Input.Description)
                .NotEmpty().WithMessage("Preencha a Descrição")
                .MaximumLength(2000).WithMessage("Descrição deve ter no máximo 2000 caracteres")
                .MinimumLength(20).WithMessage("Descrição deve ter no mínimo 20 caracteres");
        }
    }
}
