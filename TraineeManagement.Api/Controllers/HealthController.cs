using Microsoft.AspNetCore.Mvc;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private const string Running = "Running";
        private const string ApplicationName = "Trainee Management API";

        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = Running,
                application = ApplicationName,
                timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            });
        }
    }
}
