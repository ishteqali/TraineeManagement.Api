using Microsoft.AspNetCore.Mvc;
using TrainingDirectory.Api.Models;

namespace TrainingDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/trainees")]
    public class TraineesController : ControllerBase
    {
        private readonly ILogger<TraineesController> _logger;
        public TraineesController(ILogger<TraineesController> logger)
        {
            _logger = logger;
        }
        private const string Name = "Ishteqali Khan";
        private const string Email = "ishteqali.khan@gmail.com";
        private const string Department = "Developer";
        private const bool IsActive = true;
        [HttpGet("{id:int}/{correlationId}")]
        public ActionResult<TraineeProfile> Get(int id, string correlationId)
        {
            _logger.LogInformation("Correlation id received ID: {id}", correlationId);

            TraineeProfile trainee = new()
            {
                Id = id,
                Name = Name,
                Email = Email,
                Department = Department,
                IsActive = IsActive
            };
            _logger.LogInformation("Trainee retrived successfully");
            return Ok(trainee);
        }
    }
}

