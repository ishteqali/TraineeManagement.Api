using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/training-directory")]
    public class TrainingDirectoryController : ControllerBase
    {
        private readonly ITrainingDirectoryClient _trainingDirectoryClient;

        public TrainingDirectoryController(ITrainingDirectoryClient trainingDirectoryClient)
        {
            _trainingDirectoryClient = trainingDirectoryClient;
        }

        [HttpGet("trainees/{id:int}")]
        public async Task<IActionResult> GetTrainee(int id, CancellationToken cancellationToken)
        {
            string correlationId = HttpContext.TraceIdentifier;

            TraineeProfileResponse? trainee = await _trainingDirectoryClient.GetTraineeAsync(id, correlationId, cancellationToken);
            if (trainee == null)
            {
                ErrorResponse errorResponse = new ErrorResponse
                {
                    StatusCode = StatusCodes.Status503ServiceUnavailable,
                    Message = ExceptionMessages.TrainingDirectoryNotAvailable,
                    Timestamp = DateTime.UtcNow
                };
                return StatusCode(StatusCodes.Status503ServiceUnavailable, errorResponse);
            }
            return Ok(trainee);
        }
    }
}

