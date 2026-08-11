using System.Net;
using System.Net.Http.Json;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Services
{
    public class GenericHttpClient : IGenericHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GenericHttpClient> _logger;

        public GenericHttpClient(HttpClient httpClient, ILogger<GenericHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<TResponse?> GetAsync<TResponse>(string endpoint, string? correlationId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, endpoint);

                AddCorrelationId(request, correlationId);

                using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTTP GET request failed. Endpoint: {Endpoint}, CorrelationId: {CorrelationId}", endpoint, correlationId);
                throw;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestBody, string? correlationId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Post, endpoint)
                {
                    Content = JsonContent.Create(requestBody)
                };

                AddCorrelationId(request, correlationId);

                using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTTP POST request failed. Endpoint: {Endpoint}, CorrelationId: {CorrelationId}", endpoint, correlationId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, string? correlationId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Delete, endpoint);

                AddCorrelationId(request, correlationId);

                using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTTP DELETE request failed. Endpoint: {Endpoint}, CorrelationId: {CorrelationId}", endpoint, correlationId);
                return false;
            }
        }

        private static void AddCorrelationId(HttpRequestMessage request, string? correlationId)
        {
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                request.Headers.Add("X-Correlation-ID", correlationId);
            }
        }
    }
}

