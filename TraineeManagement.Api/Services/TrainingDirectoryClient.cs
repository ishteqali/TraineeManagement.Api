using System.Net;
using System.Net.Http.Json;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Services
{
    public class TrainingDirectoryClient : ITrainingDirectoryClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TrainingDirectoryClient> _logger;

        public TrainingDirectoryClient(HttpClient httpClient, ILogger<TrainingDirectoryClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        private const string BaseApi = "api/trainees/";

        public async Task<TraineeProfileResponse?> GetTraineeAsync(int traineeId, string correlationId, CancellationToken cancellationToken)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, $"{BaseApi}{traineeId}");
                request.Headers.Add("X-Correlation-ID", correlationId);

                using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<TraineeProfileResponse>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Training Directory service unavailable.");
                return null;
            }
        }
    }
}

