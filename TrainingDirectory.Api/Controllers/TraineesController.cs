using Microsoft.AspNetCore.Mvc;
using TrainingDirectory.Api.Models;

namespace TrainingDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/trainees")]
    public class TraineesController : ControllerBase
    {
        private const string Name = "Ishteqali Khan";
        private const string Email = "ishteqali.khan@gmail.com";
        private const string Department = "Developer";
        private const bool IsActive = true;
        [HttpGet("{id:int}")]
        public ActionResult<TraineeProfile> Get(int id)
        {
            TraineeProfile trainee = new()
            {
                Id = id,
                Name = Name,
                Email = Email,
                Department = Department,
                IsActive = IsActive
            };

            return Ok(trainee);
        }
    }
}

