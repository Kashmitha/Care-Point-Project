using Microsoft.EntityFrameworkCore;
using CarePoint.API.Data;
using CarePoint.API.Interfaces;
using CarePoint.API.DTOs.Admin;

namespace CarePoint.API.Services
{
    public class AdminService : IAdminService
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(ApplicationDbContext context, ILogger<AdminService> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<AdminStatsDto> GetDashboardStatsAsync()
        {
            try
            {
                var today = DateTime.Today;

                // Get user counts
                var totalUsers = await _context.Users.CountAsync();
                var totalPatients = await _context.Users.CountAsync(u => u.Role == "Patient");
                var totalDoctors = await _context.Doctors.CountAsync();
                var approvedDoctors = await _context.Doctors.CountAsync(d => d.IsApproved);
                var pendingDoctors = await _context.Doctors.CountAsync(d => !d.IsApproved);

                // Get appointment counts
                var totalAppointments = await _context.Appointments.CountAsync();
                var todayAppointments = await _context.Appointments
                    .CountAsync(a => a.AppointmentDate == today && a.Status == "Scheduled");
                var completedAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == "Completed");
                var cancelledAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == "Cancelled");

                // Get specialty statistics
                var specialtyStats = await _context.Specialties
                    .Select(s => new SpecialtyStatsDto
                    {
                        SpecialtyName = s.SpecialtyName,
                        DoctorCount = s.Doctors.Count(d => d.IsApproved),
                        AppointmentCount = s.Doctors
                            .SelectMany(d => d.Appointments)
                            .Count()
                    })
                    .ToListAsync();

                return new AdminStatsDto
                {
                    TotalUsers = totalUsers,
                    TotalPatients = totalPatients,
                    TotalDoctors = totalDoctors,
                    ApprovedDoctors = approvedDoctors,
                    PendingDoctors = pendingDoctors,
                    TotalAppointments = totalAppointments,
                    TodayAppointments = todayAppointments,
                    CompletedAppointments = completedAppointments,
                    CancelledAppointments = cancelledAppointments,
                    SpecialtyStats = specialtyStats
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard stats");
                throw;
            }
        }

        public async Task<List<DoctorApprovalDto>> GetPendingDoctorsAsync()
        {
            try
            {
                var pendingDoctors = await _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Specialty)
                    .Where(d => !d.IsApproved)
                    .OrderBy(d => d.CreatedAt)
                    .Select(d => new DoctorApprovalDto
                    {
                        DoctorId = d.DoctorId,
                        UserId = d.UserId,
                        FirstName = d.User.FirstName,
                        LastName = d.User.LastName,
                        Email = d.User.Email,
                        PhoneNumber = d.User.PhoneNumber,
                        LicenseNumber = d.LicenseNumber,
                        SpecialtyName = d.Specialty != null ? d.Specialty.SpecialtyName : null,
                        YearsOfExperience = d.YearsOfExperience,
                        Qualification = d.Qualification,
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return pendingDoctors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending doctors");
                throw;
            }
        }

        public async Task<bool> ApproveDoctorAsync(int doctorId, int adminId)
        {
            try
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

                if (doctor == null)
                {
                    return false;
                }

                if (doctor.IsApproved)
                {
                    throw new InvalidOperationException("Doctor is already approved");
                }

                doctor.IsApproved = true;
                doctor.ApprovedBy = adminId;
                doctor.ApprovedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Doctor {doctorId} approved by admin {adminId}");

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving doctor: d{doctorId}");
                throw;
            }
        }

        public async Task<bool> RejectDoctorAsync(int doctorId)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

                if (doctor == null)
                {
                    return false;
                }

                if (doctor.IsApproved)
                {
                    throw new InvalidOperationException("Cannot reject approved doctor");
                }

                // Remove doctor and associated user
                _context.Doctors.Remove(doctor);
                _context.Users.Remove(doctor.User);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Doctor {doctorId} rejected and removed");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting doctor: {doctorId}");
                throw;
            }
        }
    }
}