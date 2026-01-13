using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CarePoint.API.DTOs.Appointment;
using CarePoint.API.Interfaces;
using CarePoint.API.Data;

namespace CarePoint.API.Controllers
{
    ///
    /// Controller for appointment management
    /// 
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        ///
        /// Create new appointment (Patient only)
        /// POST: api/appointments
        /// 
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var appointment = await _appointmentService.CreateAppointmentAsync(userId, dto);

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.AppointmentId }, appointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, new { message = "An error occurred while creating appointment" });
            }
        }

        ///
        /// Get appointment by ID
        /// GET: api/Appointments/{id}
        /// 
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                // Authorization check (Only patient, doctor, and admin can view)
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != "Admin" &&
                    appointment.PatientId != userId &&
                    !IsAppointmentDoctor(appointment, userId))
                {
                    return Forbid();
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting appointment: {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving appointment" });
            }
        }

        /// <summary>
        /// Get patient's appointments
        /// GET: api/Appointments/patient
        /// </summary>
        /// <returns></returns>
        [HttpGet("patient")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<Task<AppointmentDto>>> GetPatientAppointments()
        {
            try
            {
                var userId = GetCurrentUserId();
                var appointments = await _appointmentService.GetPatientAppointmentsAsync(userId);

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving patient appointments" });
            }
        }

        /// <summary>
        /// Get doctor's appointments
        /// GET: api/Appointments/doctor    
        /// </summary>
        /// <returns></returns>
        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<List<AppointmentDto>>> GetDoctorAppointments()
        {
            try
            {
                // This assume doctorId = userId for simplicity
                var userId = GetCurrentUserId();
                var appointments = await _appointmentService.GetDoctorAppointmentsAsync(userId);

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving doctor appointments" });
            }
        }

        /// <summary>
        /// Update appointment (Patient only, Can only update their own appointments)
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto dto)
        {
            try
            {
                // Verify appointment belongs to user
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                var userId = GetCurrentUserId();
                if (appointment.PatientId != userId)
                {
                    return Forbid();
                }

                var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(id, dto);
                return Ok(updatedAppointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment: {id}");
                return StatusCode(500, new { message = "An error occurred while updating appointment" });
            }
        }

        /// <summary>
        /// Cancel appointment
        /// POST: api/Appointments/{id}/cancel
        /// </summary>
        /// <returns></returns>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentDto dto)
        {
            try
            {
                // Verify appointment belongs to user or user is the doctor
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (userRole != "Admin" &&
                    appointment.PatientId != userId &&
                    !IsAppointmentDoctor(appointment, userId))
                {
                    return Forbid();
                }

                var result = await _appointmentService.CancelAppointmentAsync(id, userId, dto);

                if (!result)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error canceling appointment: {id}");
                return StatusCode(500, new { message = "An error occurred while canceling appointment" });
            }
        }

        /// <summary>
        /// Complete appointment (Doctor only)
        /// POST: api/Appointments/{id}/complete
        /// </summary>
        /// <returns></returns>
        [HttpPost("{id}/complete")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> CompleteAppointment(int id, [FromBody] CompleteAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.CompleteAppointmentAsync(id, dto);
                if (!result)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing appointment: {id}");
                return StatusCode(500, new { message = "An error occurred while completing appointment" });
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

        private bool IsAppointmentDoctor(AppointmentDto appointmentDto, int userId)
        {
            return false; // If we need to check user correspond to the doctor in production, we implement this method
        }
    }
}