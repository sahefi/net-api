using Microsoft.AspNetCore.Mvc;
using net_api.DTOs;
using net_api.Services;

namespace net_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            try
            {
                var result = await _authService.LoginAsync(request);
                if (result == null)
                {
                    return Unauthorized(ApiResponse<LoginResponse>.Unauthorized("Invalid username or password"));
                }

                return Ok(ApiResponse<LoginResponse>.Success(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LoginResponse>.BadRequest(ex.Message));
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(ApiResponse<List<UserResponse>>.Success(users, "Users retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<UserResponse>>.BadRequest(ex.Message));
            }
        }
    }
}
