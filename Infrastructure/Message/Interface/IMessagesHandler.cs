namespace Infrastructure.Message.Interface
{
    public interface IMessagesHandler
    {
        Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command) where TCommand : Command<TResponse>;
        Task SendDomainNotificationAsync<TDomainNotification>(TDomainNotification domainNotification) where TDomainNotification : DomainNotification;
    }
}
