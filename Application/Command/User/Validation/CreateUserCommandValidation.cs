using Application.Boudary.Post;
using FluentValidation;

namespace Application.Command.User.Validation
{
    public class CreateUserCommandValidation : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidation()
        {
            RuleFor(d => d.Input.Name)
                .NotEmpty().WithMessage("Preencha o Nome")
                .MaximumLength(50).WithMessage("Nome deve ter no máximo 50 caracteres")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres");

            RuleFor(d => d.Input.Email)
           .NotEmpty().WithMessage("Insira um email.")
           .Matches(@"[a-z0-9]+@[a-z]+\.[a-z]{2,3}")
           .WithMessage("Insira um email válido ex: [ seuemail@email.com ] ");

            RuleFor(u => u.Input.Password)
              .NotEmpty()
              .WithMessage("Obrigatório informar a senha")
              .MinimumLength(6)
              .WithMessage("Senha deve conter no mínimo 6 carácteres")
              .MaximumLength(14)
              .WithMessage("Senha deve conter no máximo 14 carácteres")
              .Must(x =>
              {
                  return x.Any(a => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(a));
              }).WithMessage("Senha deve conter pelo menos 1 letra maiúscula")
              .Must(x =>
              {
                  return x.Any(a => "abcdefghijklmnopqrstuvwxyz".Contains(a));
              }).WithMessage("Senha deve conter pelo menos 1 letra minúscula")
              .Must(x =>
              {
                  return x.Any(a => "0123456789".Contains(a));
              }).WithMessage("Senha deve conter pelo menos 1 número")
              .Must(x =>
              {
                  return x.Any(a => "!@#$%¨&*()_+".Contains(a));
              }).WithMessage("Senha deve conter pelo menos 1 carácter especial (Ex: !@#$%¨&*()_+)");


            RuleFor(d => d.Input.PasswordConfirmation)
                .NotEmpty().WithMessage("Confirme a senha.");

            RuleFor(d => d.Input.Password)
                .Equal(d => d.Input.PasswordConfirmation)
                .WithMessage("As senhas não coincidem.");
        }
    }
}
