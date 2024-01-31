using MediatR;
using FluentValidation.Results;

namespace Infrastructure.Message
{
    public abstract class Command<TResponse> : Message, IRequest<TResponse>
    {
        public DateTime TimeStamp { get; set; }
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public abstract bool IsValid();
    }
}
