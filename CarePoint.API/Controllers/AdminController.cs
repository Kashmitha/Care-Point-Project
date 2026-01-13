using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CarePoint.API.DTOs.Admin;
using CarePoint.API.Interfaces;

namespace CarePoint.API.Controllers
{
    /// 
    /// Controller for admin operations
    /// 
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        ///
        /// Get dashboard statistics
        /// GET: api/Admin/stats
        /// 
        [HttpGet("stats")]
        public async Task<ActionResult<AdminStatsDto>> GetDashboardStats()
        {
            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, new { message = "An error occurred while retrieving statistics" });
            }
        }

        ///
        /// Get pending doctor approvals
        /// GET: api/Admin/pending-doctors
        /// 
        [HttpGet("pending-doctors")]
        public async Task<ActionResult<List<DoctorApprovalDto>>> GetPendingDoctors()
        {
            try
            {
                var pendingDoctors = await _adminService.GetPendingDoctorsAsync();
                return Ok(pendingDoctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending doctors");
                return StatusCode(500, new { message = "An error occurred while retrieving pending doctors" });
            }
        }

        ///
        /// Approve doctor registration
        /// POST: api/Admin/approve-doctor/{doctorId}
        /// 
        [HttpPost("approve-doctor/{doctorId}")]
        public async Task<ActionResult> ApproveDoctor(int doctorId)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var result = await _adminService.ApproveDoctorAsync(doctorId, adminId);

                if (!result)
                {
                    return NotFound(new { message = "Doctor not found" });
                }

                return Ok(new { message = "Doctor approved successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving doctor: {doctorId}");
                return StatusCode(500, new { message = "An error occurred while approving doctor" });
            }
        }

        /// 
        /// Reject doctor registration
        /// POST: api/Admin/reject-doctor/{doctorId}
        /// 
        [HttpPost("reject-doctor/{doctorId}")]
        public async Task<ActionResult> RejectDoctor(int doctorId)
        {
            try
            {
                var result = await _adminService.RejectDoctorAsync(doctorId);

                if (!result)
                {
                    return NotFound(new { message = "Doctor not found" });
                }

                return Ok(new { message = "Doctor rejected successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting doctor: {doctorId}");
                return StatusCode(500, new { message = "An error occurred while rejecting doctor" });
            }
        }

        // Helper method
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}