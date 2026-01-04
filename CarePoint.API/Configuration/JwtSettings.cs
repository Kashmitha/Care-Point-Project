namespace CarePoint.API.Configuration
{
    // Configuration settings for JWT authentication.
    public class JwtSettings
    {
        // Secret key used for signing JWT tokens.
        public string SecretKey { get; set; } = string.Empty;

        // Issuer of the JWT token.
        public string Issuer { get; set; } = string.Empty;

        // Audience for the JWT token.
        public string Audience { get; set; } = string.Empty;

        // Token expiration time in minutes.
        public int ExpiryInMinutes { get; set; }
    }
}