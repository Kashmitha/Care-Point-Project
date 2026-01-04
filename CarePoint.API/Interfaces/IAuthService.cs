using CarePoint.API.DTOs.Auth;

namespace CarePoint.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> EmailExistsAsync(string email);
    }
}