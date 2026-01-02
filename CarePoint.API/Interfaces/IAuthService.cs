using CarePoint.API.DTOs.Auth;

namespace CarePoint.API.Interface
{
    // Inteface enable DI and testability.
    // Makes it easy to mock services in unit tests.
    public interface IAuthService
    {
        // Why we use interface
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> EmailExistsAsync(string email);
    }
}