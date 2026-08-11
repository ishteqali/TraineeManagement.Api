namespace TraineeManagement.Api.Interfaces;

public interface IGenericHttpClient
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, string? correlationId = null, CancellationToken cancellationToken = default);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, string? correlationId = null, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string endpoint, string? correlationId = null, CancellationToken cancellationToken = default);
}