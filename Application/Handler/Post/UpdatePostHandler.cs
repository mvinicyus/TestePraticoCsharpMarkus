using Application.Boudary.Post;
using Application.Command.Post;
using Domain.Entity.User;
using Domain.Interface.Cryptography;
using Infrastructure.Message;
using Infrastructure.Message.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Security.Claims;

namespace Application.Handler.Post
{
    public class UpdatePostHandler : IRequestHandler<UpdatePostCommand, UpdatePostOutput>
    {
        private readonly IMessagesHandler _messagesHandler;
        private readonly GenericRepository<UserEntity, int> _userRepository;
        private readonly GenericRepository<PostEntity, int> _postRepository;
        private readonly ISha _sha;
        private readonly IHttpContextAccessor _accessor;

        public UpdatePostHandler(IMessagesHandler messagesHandler,
                           GenericRepository<UserEntity, int> userRepository,
                           ISha sha,
                           GenericRepository<PostEntity, int> postRepository,
                           IHttpContextAccessor accessor)
        {
            _messagesHandler = messagesHandler;
            _userRepository = userRepository;
            _sha = sha;
            _postRepository = postRepository;
            _accessor = accessor;
        }

        private int GetUserIdFromJwtToken()
        {
            var claims = _accessor?.HttpContext?.User?.Claims;
            var claimId = claims?.FirstOrDefault(a => a?.Type == ClaimTypes.PrimarySid).Value ?? "";
            int id = 0;
            if (int.TryParse(claimId, out id))
            {
                return id;
            }
            return 0;
        }

        public async Task<UpdatePostOutput> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
        {
            if (command.IsValid())
            {
                await _userRepository
                       .BeginTransactionAsync(false)
                       .ConfigureAwait(false);

                var sessionId = GetUserIdFromJwtToken();

                var loggedInUser = await _userRepository
                     .DbSet
                     .FirstOrDefaultAsync(user => user.Active && user.Id == sessionId);

                if (loggedInUser == null)
                {
                    _ = ApplyErrorAsync("Por favor! faça login.");
                    return null;
                }

                var post = await _postRepository
                    .DbSet
                    .FirstOrDefaultAsync(post => post.Id == command.Input.Id)
                    .ConfigureAwait(false);

                if (post == null)
                {
                    _ = ApplyErrorAsync("Post não encontrado.");
                    return null;
                }

                post.Title = command.Input.Title;
                post.Description = command.Input.Description;
                post.UpdateDate = DateTime.UtcNow;

                await _userRepository.CommitTransactionAsync();

                return new UpdatePostOutput();
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
