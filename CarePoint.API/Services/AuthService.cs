using Microsoft.EntityFrameworkCore;
using CarePoint.API.Data;
using CarePoint.API.DTOs.Auth;
using CarePoint.API.Interfaces;
using CarePoint.API.Models;

namespace CarePoint.API.services
{
    // Service for handling user authentication and registration
    // Never stores plain text password
    // Use strong hashing BCrypt
    // Implement rate limitting for login attempts 
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IJwtService jwtService, IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration; 
        }

        public async Task RegisterAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            if(await EmailExistsAsync(registerDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Hash password using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create user
            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                DateOfBirth = registerDto.DateOfBirth,
                Gender = registerDto.Gender,
                Address = registerDto.Address,
                City = registerDto.City,
                State = registerDto.State,
                ZipCode = registerDto.ZipCode,
                Role = registerDto.Role,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // If registering as doctor, create doctor profile
            if(registerDto.Role == "Doctor" && !string.IsNullOrEmpty(registerDto.LicenseNumber))
            {
                var doctor = new Doctor
                {
                    UserId = user.UserId,
                    LicenseNumber = registerDto.LicenseNumber,
                    SpecialtyId = registerDto.SpecialtyId,
                    YearsOfExperience = registerDto.YearsOfExperience,
                    Qualification = registerDto.Qualification,
                    Bio = registerDto.Bio,
                    ConsultationFee = registerDto.ConsultationFee,
                    IsApproved = false // Requires admin approval
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);
            var expiryMinutes = _configuration.GetValue("JwtSettings:ExpiryInMinutes");

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes) 
            };
        }

        public async Task LoginAsync(LoginDto loginDto)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefautAsync(u => u.Email == loginDto.Email);

            if(user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Verify password
            if(!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or passowrd");
            }

            // Check if user is active
            if(!user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is inactive");
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);
            var expiryMinutes = _configuration.GetValue("JwtSettings:ExpiryInMinutes");

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };
        }

        public async Task EmailExistsAsyn(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}