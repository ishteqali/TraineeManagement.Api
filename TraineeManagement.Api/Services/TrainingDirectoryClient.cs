using System.Net;
using System.Net.Http.Json;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Services
{
    public class TrainingDirectoryClient : ITrainingDirectoryClient
    {
        private readonly IGenericHttpClient _httpClient;
        private readonly ILogger<TrainingDirectoryClient> _logger;

        public TrainingDirectoryClient(IGenericHttpClient httpClient, ILogger<TrainingDirectoryClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        private const string BaseApi = "api/trainees/";

        public async Task<TraineeProfileResponse?> GetTraineeAsync(int traineeId, string correlationId, CancellationToken cancellationToken)
        {
            return await _httpClient.GetAsync<TraineeProfileResponse>($"{BaseApi}{traineeId}/{correlationId}", correlationId, cancellationToken);
        }
    }
}

