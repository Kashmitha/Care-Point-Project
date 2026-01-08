using Microsoft.EntityFrameworkCore;
using CarePoint.API.Data;
using CarePoint.API.DTOs.Appointment;
using CarePoint.API.Interfaces;
using CarePoint.API.Models;
using System.Xml;

namespace CarePoint.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentService> _logger;
        private readonly IEmailService _emailService;

        public AppointmentService(ApplicationDbContext context, IEmailService emailService, ILogger<AppointmentService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto)
        {
            try
            {
                // Validate patient exists
                var patient = await _context.Users.FindAsync(patientId);

                if (patient == null || patient.Role != "Patient")
                {
                    throw new InvalidOperationException("Invalid patient");
                }

                // Validate doctor exists and is approved 
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Specialty)
                    .FirstOrDefaultAsync(d => d.DoctorId == dto.DoctorId && d.IsApproved);

                if (doctor == null)
                {
                    throw new InvalidOperationException("Doctor not found or not approved");
                }

                // Validate operation date is in future
                var appointmentDateTime = dto.AppointmentDate.Date.Add(dto.AppointmentTime);

                if (appointmentDateTime <= DateTime.Now)
                {
                    throw new InvalidOperationException("Appointment must be in the future");
                }

                // Check if doctor is available
                var dayOfWeek = (int)dto.AppointmentDate.DayOfWeek;
                var isAvailable = await _context.DoctorAvailabilities
                    .AnyAsync(da =>
                        da.DoctorId == dto.DoctorId &&
                        da.DayOfWeek == dayOfWeek &&
                        da.IsActive &&
                        dto.AppointmentTime >= da.StartTime &&
                        dto.AppointmentTime < da.EndTime);

                if (!isAvailable)
                {
                    throw new InvalidOperationException("Doctor is not available at this time");
                }

                // Check for conflicts
                var hasConflict = await _context.Appointments
                    .AnyAsync(a =>
                        a.DoctorId == dto.DoctorId &&
                        a.AppointmentDate == dto.AppointmentDate &&
                        a.AppointmentTime == dto.AppointmentTime &&
                        a.Status != "Cancelled");

                if (hasConflict)
                {
                    throw new InvalidOperationException("This time slot is already booked");
                }

                // Create appointment
                var appointment = new Appointment
                {
                    PatientId = patientId,
                    DoctorId = dto.DoctorId,
                    AppointmentDate = dto.AppointmentDate,
                    AppointmentTime = dto.AppointmentTime,
                    ReasonForVisit = dto.ReasonForVisit,
                    Status = "Scheduled"
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // Send confirmation email
                try
                {
                    await _emailService.SendAppointmentConfirmationAsync(
                        patient.Email,
                        $"{patient.FirstName} {patient.LastName}",
                        $"{doctor.User.FirstName} {doctor.User.LastName}",
                        dto.AppointmentDate,
                        dto.AppointmentTime);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send appointment confirmation email");
                }

                return new AppointmentDto
                {
                    AppointmentId = appointment.AppointmentId,
                    PatientId = appointment.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    DoctorId = appointment.DoctorId,
                    DoctorName = $"{doctor.User.FirstName} {doctor.User.LastName}",
                    SpecialtyName = doctor.Specialty?.SpecialtyName,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentTime = appointment.AppointmentTime,
                    Status = appointment.Status,
                    ReasonForVisit = appointment.ReasonForVisit,
                    Notes = appointment.Notes,
                    CreatedAt = appointment.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                throw;
            }
        }

        public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(int patientId)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Doctor).ThenInclude(d => d.User)
                    .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                    .Include(a => a.Patient)
                    .Where(a => a.PatientId == patientId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .Select(a => new AppointmentDto
                    {
                        AppointmentId = a.AppointmentId,
                        PatientId = a.PatientId,
                        PatientName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                        DoctorId = a.DoctorId,
                        DoctorName = $"{a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                        SpecialtyName = a.Doctor.Specialty != null ? a.Doctor.Specialty.SpecialtyName : null,
                        AppointmentDate = a.AppointmentDate,
                        AppointmentTime = a.AppointmentTime,
                        Status = a.Status,
                        ReasonForVisit = a.ReasonForVisit,
                        Notes = a.Notes,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting patient appointments: {patientId}");
                throw;
            }
        }

        public async Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Doctor).ThenInclude(d => d.User)
                    .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorId == doctorId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .Select(a => new AppointmentDto
                    {
                        AppointmentId = a.AppointmentId,
                        PatientId = a.PatientId,
                        PatientName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                        DoctorId = a.DoctorId,
                        DoctorName = $"{a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                        SpecialtyName = a.Doctor.Specialty != null ? a.Doctor.Specialty.SpecialtyName : null,
                        AppointmentDate = a.AppointmentDate,
                        AppointmentTime = a.AppointmentTime,
                        Status = a.Status,
                        ReasonForVisit = a.ReasonForVisit,
                        Notes = a.Notes,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctor appointments: {doctorId}");
                throw;
            }
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Doctor).ThenInclude(d => d.User)
                    .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                    .Include(a => a.Patient)
                    .Where(a => a.AppointmentId == appointmentId)
                    .Select(a => new AppointmentDto
                    {
                        AppointmentId = a.AppointmentId,
                        PatientId = a.PatientId,
                        PatientName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                        DoctorId = a.DoctorId,
                        DoctorName = $"{a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                        SpecialtyName = a.Doctor.Specialty != null ? a.Doctor.Specialty.SpecialtyName : null,
                        AppointmentDate = a.AppointmentDate,
                        AppointmentTime = a.AppointmentTime,
                        Status = a.Status,
                        ReasonForVisit = a.ReasonForVisit,
                        Notes = a.Notes,
                        CreatedAt = a.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                return appointment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting appointment by ID: {appointmentId}");
                throw;
            }
        }

        public async Task<AppointmentDto> UpdateAppointmentAsync(int appointmentId, UpdateAppointmentDto dto)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment == null)
                {
                    throw new InvalidOperationException("Appointment not found");
                }

                if (appointment.Status != "Scheduled")
                {
                    throw new InvalidOperationException("Can only update scheduled appointments");
                }

                // Validate new time is in future
                var newDateTime = dto.AppointmentDate.Date.Add(dto.AppointmentTime);

                if (newDateTime <= DateTime.Now)
                {
                    throw new InvalidOperationException("Appointment must be in the future");
                }

                // Check for conflicts (excluding current appointment)
                var hasConflict = await _context.Appointments
                    .AnyAsync(a =>
                        a.AppointmentId != appointmentId &&
                        a.DoctorId == appointment.DoctorId &&
                        a.AppointmentDate == dto.AppointmentDate &&
                        a.AppointmentTime == dto.AppointmentTime &&
                        a.Status != "Cancelled");

                if (hasConflict)
                {
                    throw new InvalidOperationException("This time slot is already booked");
                }

                appointment.AppointmentDate = dto.AppointmentDate;
                appointment.AppointmentTime = dto.AppointmentTime;
                appointment.ReasonForVisit = dto.ReasonForVisit;
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return await GetAppointmentByIdAsync(appointmentId)
                    ?? throw new InvalidOperationException("Failed to retrieve updated appointment");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment: {appointmentId}");
                throw;
            }
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, int userId, CancelAppointmentDto dto)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment == null)
                {
                    return false;
                }

                if (appointment.Status != "Scheduled")
                {
                    throw new InvalidOperationException("Can only cancel scheduled appointments");
                }

                appointment.Status = "Cancelled";
                appointment.CancelledBy = userId;
                appointment.CancellationReason = dto.CancellationReason;
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling appointment: {appointmentId}");
                throw;
            }
        }

        public async Task<bool> CompleteAppointmentAsync(int appointmentId, CompleteAppointmentDto dto)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment == null)
                {
                    return false;
                }

                if (appointment.Status != "Scheduled")
                {
                    throw new InvalidOperationException("Can only complete scheduled appointments");
                }

                appointment.Status = "Completed";
                appointment.Notes = dto.Notes;
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing appointment: {appointmentId}");
                throw;
            }
        }
    }
}