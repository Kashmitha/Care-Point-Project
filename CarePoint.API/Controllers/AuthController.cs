using Microsoft.AspNetCore.Mvc;
using CarePoint.API.DTOs.Auth;
using CarePoint.API.Interfaces;

namespace CarePoint.API.Controllers
{
    // Controller handling user authentication endpoints
    [Route("api/[controller]")]
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

        // Register a new user (Patient or Doctor)
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try {
                var response = await _authService.RegisterAsync(registerDto);
                return Ok(response);
            }
            catch(InvalidOperationException ex) {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new { message = "An error occurred during the registration"});
            }
        }

        // Login user and return JWT token
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            try {
                var response = await _authService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex) {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login"});
            }
        }

        // Check if email already registered
        [HttpGet("check-email")]
        public async Task<ActionResult> CheckEmail([FromQuery] string email)
        {
            var exists = await _authService.EmailExistsAsync(email);
            return Ok(new { exists });
        }
    }
}