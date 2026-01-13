using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarePoint.API.Data;
using CarePoint.API.DTOs.Doctor;
using CarePoint.API.Interfaces;
using System.Security.Claims;

namespace CarePoint.API.Controllers
{
    ///
    /// Controller for doctor related operations
    /// 
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorsController> _logger;
        public DoctorsController(IDoctorService doctorService, ApplicationDbContext context, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _context = context;
            _logger = logger;
        }

        ///
        /// Search for doctors with filters
        /// GET: api/Doctors/search
        /// 
        [HttpGet("search")]
        public async Task<ActionResult<List<DoctorProfileDto>>> SearchDoctors([FromQuery] DoctorSearchDto searchDto)
        {
            try
            {
                var doctors = await _doctorService.SearchDoctorsAsync(searchDto);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors");
                return StatusCode(500, new { message = "An error occurred while searching doctors" });
            }
        }

        ///
        /// Get doctor profile by ID
        /// GET: api/Doctors/{id}
        /// 
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorProfileDto>> GetDoctorProfile(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorProfileAsync(id);

                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found " });
                }

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctor profile: {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving doctor profile" });
            }
        }

        /// 
        /// Get doctor's availability schedule
        /// GET: api/Doctors/{id}/availability
        /// 
        [HttpGet("{id}/availability")]
        public async Task<ActionResult<List<DoctorAvailabilityDto>>> GetDoctorAvailability(int id)
        {
            try
            {
                var availability = await _doctorService.GetDoctorAvailabilityAsync(id);
                return Ok(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctor availability: {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving availability" });
            }
        }

        ///
        /// Add availability slot for doctor (Doctor only)
        /// POST: api/Doctors/availability
        /// 
        [HttpPost("availability")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<DoctorAvailabilityDto>> AddAvailability([FromBody] CreateAvailabilityDto dto)
        {
            try
            {
                // Get doctor ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user identifier" });
                }

                // Get doctor record for this user
                var doctorId = await GetDoctorIdFromUserId(userId);
                if (doctorId == null)
                {
                    return NotFound(new { message = "Doctor profile not found" });
                }

                var availability = await _doctorService.AddAvailabilityAsync(doctorId.Value, dto);
                return CreatedAtAction(nameof(GetDoctorAvailability), new { id = doctorId.Value }, availability);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding availability");
                return StatusCode(500, new { message = "An error occurred while adding availability" });
            }
        }

        ///
        /// Delete availability slot (Doctor only)
        /// DELETE: api/Doctors/availability/{id}
        /// 
        [HttpDelete("availability/{availabilityId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteAvailability(int availabilityId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user identifier" });
                }

                var doctorId = await GetDoctorIdFromUserId(userId);
                if (doctorId == null)
                {
                    return NotFound(new { message = "Doctor profile not found" });
                }

                var result = await _doctorService.DeleteAvailabilityAsync(doctorId.Value, availabilityId);

                if (!result)
                {
                    return NotFound(new { message = "Availability not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting availability: {availabilityId}");
                return StatusCode(500, new { message = "An error occurred while deleting availability" });
            }
        }

        ///
        /// Update doctor profile (Doctor only)
        /// PUT: api/Doctors/profile
        /// 
        [HttpPut("profile")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateDoctorProfileDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var doctorId = await GetDoctorIdFromUserId(int.Parse(userIdClaim));
                if (doctorId == null)
                {
                    return NotFound(new { message = "Doctor profile not found" });
                }

                var updateProfile = await _doctorService.UpdateDoctorProfileAsync(doctorId.Value, dto);
                return Ok(updateProfile);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor profile");
                return StatusCode(500, new { message = "An error occurred while updating profile" });
            }
        }

        // Helper method
        private async Task<int?> GetDoctorIdFromUserId(int userId)
        {
            return await _context.Doctors
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .Select(d => (int?)d.DoctorId)
                .FirstOrDefaultAsync();
        }
    }
}