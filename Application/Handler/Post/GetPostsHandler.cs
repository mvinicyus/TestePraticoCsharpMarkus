using Application.Boudary.Post;
using Application.Command.Post;
using Domain.Entity.User;
using Infrastructure.Message;
using Infrastructure.Message.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Application.Handler.Post
{
    public class GetPostsHandler : IRequestHandler<GetPostsCommand, GetPostsOutput>
    {
        private readonly IMessagesHandler _messagesHandler;
        private readonly GenericRepository<UserEntity, int> _userRepository;
        private readonly GenericRepository<PostEntity, int> _postRepository;

        public GetPostsHandler(IMessagesHandler messagesHandler,
                           GenericRepository<UserEntity, int> userRepository,
                           GenericRepository<PostEntity, int> postRepository)
        {
            _messagesHandler = messagesHandler;
            _userRepository = userRepository;
            _postRepository = postRepository;
        }

        public async Task<GetPostsOutput> Handle(GetPostsCommand command, CancellationToken cancellationToken)
        {
            if (command.IsValid())
            {
                await _userRepository
                       .BeginTransactionAsync(true)
                       .ConfigureAwait(false);

                var startIndex = command?.Input?.StartIndex ?? 0;
                var length = command?.Input?.PageLength ?? 5;

                var query = _postRepository
                    .DbSet
                    .Where(post => post.Active);

                var total = await query
                    .CountAsync()
                    .ConfigureAwait(false);

                var posts = await query
                    .Skip(startIndex)
                    .Take(length)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (!(posts?.Any() ?? false))
                {
                    _ = ApplyErrorAsync("Posts não encontrados.");
                    return null;
                }
                await _userRepository.CommitTransactionAsync();

                return new GetPostsOutput
                {
                    Draw = command.Input.Draw.Value,
                    TotalItens = total,
                    Data = posts.Select(post => new PostInfoOutput
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Description = post.Description,
                        CreateDate = post.CreateDate.ToString("dd/MM/yyyy HH:mm"),
                        UpdateDate = post.UpdateDate?.ToString("dd/MM/yyyy HH:mm") ?? "-"
                    })
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
