using System.ComponentModel.DataAnnotations;

namespace CarePoint.API.DTOs.Auth
{
    // Response DTO after successful authentication
    // Token should be stored securely on client (httpOnly cookies or secure storage).
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}