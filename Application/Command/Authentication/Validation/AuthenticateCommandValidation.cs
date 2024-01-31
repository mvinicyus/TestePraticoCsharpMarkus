using FluentValidation;

namespace Application.Command.Authentication.Validation
{
    public class AuthenticateCommandValidation : AbstractValidator<AuthenticationCommand>
    {
        public AuthenticateCommandValidation()
        {
            RuleFor(d => d.Input.Login)
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
        }
    }
}
