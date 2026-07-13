using Microsoft.AspNetCore.Mvc;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "running",
                application = "Trainee Management API",
                timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            });
        }
    }
}
