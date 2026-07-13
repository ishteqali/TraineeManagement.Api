using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using System.Threading.Tasks;
using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }   

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResponse? response = await _authService.LoginAsync(request);
            if (response == null)
            {
                _logger.LogWarning($"Failed login attempt for username: {request.Username}");
                return Unauthorized();
            }
            _logger.LogInformation($"Successful login for username: {request.Username}");
            return Ok(response);
        }
    }
}