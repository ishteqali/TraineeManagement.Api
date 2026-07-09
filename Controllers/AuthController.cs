using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using System.Threading.Tasks;
using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }   

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResponse? response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized();
            }

            return Ok(response);
        }
    }
}