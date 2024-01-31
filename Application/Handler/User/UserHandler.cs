using Application.Boudary.User;
using Application.Command.User;
using Domain.Entity.User;
using Domain.Interface.Cryptography;
using Infrastructure.Message;
using Infrastructure.Message.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Application.Handler.User
{
    public class UserHandler : IRequestHandler<CreateUserCommand, CreateUserOutput>
    {
        private readonly IMessagesHandler _messagesHandler;
        private readonly GenericRepository<UserEntity, int> _userRepository;
        private readonly ISha _sha;

        public UserHandler(IMessagesHandler messagesHandler,
                           GenericRepository<UserEntity, int> userRepository,
                           ISha sha)
        {
            _messagesHandler = messagesHandler;
            _userRepository = userRepository;
            _sha = sha;
        }

        public async Task<CreateUserOutput> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            if (command.IsValid())
            {
                await _userRepository
                       .BeginTransactionAsync(true)
                       .ConfigureAwait(false);

                var alreadyUser = await _userRepository
                     .DbSet
                     .FirstOrDefaultAsync(d => d.Email == command.Input.Email);

                if (alreadyUser != null)
                {
                    _ = this.ApplyErrorAsync("Esse email já está cadastrado em nossa base de dados.");
                    return null;
                }

                var user = new UserEntity
                {
                    Name = command.Input.Name,
                    Email = command.Input.Email,
                    CreateDate = DateTime.UtcNow,
                    Active = true,
                    Password = _sha.Encrypt(command.Input.Password)
                };

                await _userRepository.SaveAsync(user);

                await _userRepository.CommitTransactionAsync();

                return new CreateUserOutput(user.Name, user.Email);
            }

            Parallel.ForEach(command.ValidationResult.Errors, async error =>
            {
                await ApplyErrorAsync(command.MessageType, error.ErrorMessage).ConfigureAwait(false);
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
