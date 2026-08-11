using TraineeManagement.Shared.Contracts;

namespace TraineeManagement.Api.Interfaces
{
    public interface IMessagePublisher
    {
        Task<bool> PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}

