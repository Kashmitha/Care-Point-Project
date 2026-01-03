using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CarePoint.API.Configuration;
using CarePoint.API.Configuration;
using CarePoint.API.Models;

namespace CarePoint.API.services
{
    // Service for generating JWT token
    public class JwtService : IJwtService 
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public strings GenerateToken(User user)
        {
            // Create claims (data embedded in token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRagisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Create sigining key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
