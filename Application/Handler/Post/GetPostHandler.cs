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

namespace Application.Handler.Post
{
    public class GetPostHandler : IRequestHandler<GetPostCommand, GetPostOutput>
    {
        private readonly IMessagesHandler _messagesHandler;
        private readonly GenericRepository<UserEntity, int> _userRepository;
        private readonly GenericRepository<PostEntity, int> _postRepository;
        private readonly ISha _sha;
        private readonly IHttpContextAccessor _accessor;

        public GetPostHandler(IMessagesHandler messagesHandler,
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

        public async Task<GetPostOutput> Handle(GetPostCommand command, CancellationToken cancellationToken)
        {
            if (command.IsValid())
            {
                await _userRepository
                       .BeginTransactionAsync(true)
                       .ConfigureAwait(false);

                var post = await _postRepository
                    .DbSet
                    .FirstOrDefaultAsync(post => post.Id == command.Input.Id)
                    .ConfigureAwait(false);

                if (post == null)
                {
                    _ = ApplyErrorAsync("Post não encontrado.");
                    return null;
                }

                await _userRepository.CommitTransactionAsync();

                return new GetPostOutput
                {
                    Id = post.Id,
                    Title = post.Title,
                    Description = post.Description,
                    CreateDate = post.CreateDate.ToString("dd/MM/yyyy HH:mm"),
                    UpdateDate = post.UpdateDate?.ToString("dd/MM/yyyy HH:mm") ?? "-"
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
