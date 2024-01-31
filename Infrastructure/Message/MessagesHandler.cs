using Infrastructure.Message.Interface;
using MediatR;

namespace Infrastructure.Message
{
    public class MessagesHandler : IMessagesHandler
    {
        private readonly IMediator _mediator;

        public MessagesHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command) where TCommand : Command<TResponse>
        {
            return await _mediator.Send(command).ConfigureAwait(false);
        }

        public async Task SendDomainNotificationAsync<TDomainNotification>(TDomainNotification domainNotification) where TDomainNotification : DomainNotification
        {
            await _mediator.Publish(domainNotification).ConfigureAwait(false);
        }
    }
}
