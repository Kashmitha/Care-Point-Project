using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CarePoint.API.DTOs.Prescription;
using CarePoint.API.Interfaces;

namespace CarePoint.API.Controllers
{
    ///
    /// Controller for prescription management
    /// 
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<PrescriptionsController> _logger;

        public PrescriptionsController(IPrescriptionService prescriptionService, ILogger<PrescriptionsController> logger)
        {
            _prescriptionService = prescriptionService;
            _logger = logger;
        }

        ///
        /// Create prescription (Doctor only)
        /// POST: api/prescriptions
        /// 
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> CreatePrescription([FromBody] CreatePrescriptionDto dto)
        {
            try
            {
                // Get doctor ID from user claims
                var userId = GetCurrentUserId();

                var prescription = await _prescriptionService.CreatePrescriptionAsync(userId, dto);

                return CreatedAtAction(nameof(GetPrescription), new { id = prescription.PrescriptionId }, prescription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription");
                return StatusCode(500, new { message = "An error occurred while creating prescription" });
            }
        }

        ///
        /// Get prescription by ID
        /// GET: api/Prescriptions/{id}
        /// 
        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionDto>> GetPrescription(int id)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);

                if (prescription == null)
                {
                    return NotFound(new { message = "Prescription not found" });
                }

                // Authorize check
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != "Admin" &&
                    prescription.PatientId != userId &&
                    prescription.DoctorId != userId)
                {
                    return Forbid();
                }

                return Ok(prescription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting prescription: {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving prescription" });
            }
        }

        ///
        /// Get patient's prescriptions
        /// GET: api/Prescriptions/patient
        /// 
        [HttpGet("patient")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<List<PrescriptionDto>>> GetPatientPrescriptions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var prescriptions = await _prescriptionService.GetPatientPrescriptionsAsync(userId);

                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient's prescriptions");
                return StatusCode(500, new { message = "An error occurred while retrieving patient's prescriptions" });
            }
        }

        ///
        /// Get prescription by appointment ID
        /// GET: api/Prescriptions/appointment/{appointmentId}
        /// 
        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<PrescriptionDto>> GetPrescriptionByAppointment(int appointmentId)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByAppointmentAsync(appointmentId);

                if (prescription == null)
                {
                    return NotFound(new { message = "No prescription found for this appointment" });
                }

                // Authorize check
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != "Admin" &&
                   prescription.PatientId != userId &&
                   prescription.DoctorId != userId)
                {
                    return Forbid();
                }

                return Ok(prescription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting prescription for appointment: {appointmentId}");
                return StatusCode(500, new { message = "An error occurred while retrieving prescription" });
            }
        }

        // Helper methods
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }
    }
}