using FluentValidation;

namespace Application.Command.Post.Validation
{
    public class UpdatePostCommandValidation : AbstractValidator<UpdatePostCommand>
    {
        public UpdatePostCommandValidation()
        {
            RuleFor(d => d.Input.Id)
                .NotEmpty().WithMessage("Dados incorretos")
                .GreaterThan(0).WithMessage("Dados incorretos");

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
