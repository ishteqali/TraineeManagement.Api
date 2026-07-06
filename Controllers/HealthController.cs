using Microsoft.AspNetCore.Mvc;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            var response = new
            {
                status = "running",
                application = "Trainee Management API",
                timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            return Ok(response);
        }
    }
}
