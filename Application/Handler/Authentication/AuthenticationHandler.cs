using Application.Boudary.User;
using Application.Command.Authentication;
using Domain.Entity.User;
using Domain.Interface.Cryptography;
using Infrastructure.Message;
using Infrastructure.Message.Interface;
using Infrastructure.Middleware.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Handler.Authentication
{
    public class AuthenticationHandler : IRequestHandler<AuthenticationCommand, AuthenticationOutput>
    {
        private readonly IMessagesHandler _messagesHandler;
        private readonly GenericRepository<UserEntity, int> _userRepository;
        private readonly ISha _sha;

        public AuthenticationHandler(IMessagesHandler messagesHandler,
                           GenericRepository<UserEntity, int> userRepository,
                           ISha sha)
        {
            _messagesHandler = messagesHandler;
            _userRepository = userRepository;
            _sha = sha;
        }

        public async Task<AuthenticationOutput> Handle(AuthenticationCommand command, CancellationToken cancellationToken)
        {
            if (command.IsValid())
            {
                await _userRepository
                       .BeginTransactionAsync(false)
                       .ConfigureAwait(false);

                var encryptedPassword = _sha.Encrypt(command.Input.Password);

                var alreadyUser = await _userRepository
                     .DbSet
                     .FirstOrDefaultAsync(d => d.Email == command.Input.Login);

                if (alreadyUser == null || alreadyUser?.Password != encryptedPassword)
                {
                    _ = ApplyErrorAsync("Login ou senha estão incorretos.");
                    return null;
                }
                await _userRepository.CommitTransactionAsync();

                var claims = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, alreadyUser.Email),
                    new Claim(ClaimTypes.Email, alreadyUser.Email),
                    new Claim(ClaimTypes.GivenName, alreadyUser.Name),
                    new Claim(ClaimTypes.PrimarySid, alreadyUser.Id.ToString())
                });

                var key = AuthenticationConfig.GetJwtKey();
                var expireDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(Environment.GetEnvironmentVariable("AuthenticationTimeInMinutes")));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expireDate,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var refreshToken = AuthenticationConfig.GenerateRefreshToken();


                return new AuthenticationOutput
                {
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken,
                    Login = alreadyUser.Email,
                    Name = alreadyUser.Name
                };
            }

            Parallel.ForEach(command.ValidationResult.Errors, async error =>
            {
                await ApplyErrorAsync(error.ErrorMessage, command.MessageType).ConfigureAwait(false);
            });

            return null;
        }

        private async Task<bool> ApplyErrorAsync(string message, string messageType = "error")
        {
            var isError = true;
            await _messagesHandler.SendDomainNotificationAsync
            (
                new DomainNotification
                (
                    messageType,
                    message,
                    isError
                )
            ).ConfigureAwait(false);
            return false;
        }
    }
}
