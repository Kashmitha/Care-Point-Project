using Microsoft.EntityFrameworkCore;
using CarePoint.API.Data;
using CarePoint.API.DTOs.Prescription;
using CarePoint.API.Interfaces;
using CarePoint.API.Models;

namespace CarePoint.API.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(ApplicationDbContext context, ILogger<PrescriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PrescriptionDto> CreatePrescriptionAsync(int doctorId, CreatePrescriptionDto dto)
        {
            try
            {
                // Validate appointment exists and belongs to doctor
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor).ThenInclude(d => d.User)
                    .FirstOrDefaultAsync(a => a.AppointmentId == dto.AppointmentId && a.DoctorId == doctorId);

                if (appointment == null)
                {
                    throw new InvalidOperationException("Appointment not found or does not belong to this doctor");
                }

                if (appointment.Status != "Completed")
                {
                    throw new InvalidOperationException("Can only create prescription for completed appointments");
                }

                // Check prescription already exists
                var existingPrescription = await _context.Prescriptions
                    .AnyAsync(p => p.AppointmentId == dto.AppointmentId);

                if (existingPrescription)
                {
                    throw new InvalidOperationException("Prescription already exists for this appointment");
                }

                // Create prescription
                var prescription = new Prescription
                {
                    AppointmentId = dto.AppointmentId,
                    PatientId = appointment.PatientId,
                    DoctorId = doctorId,
                    Diagnosis = dto.Diagnosis,
                    Instructions = dto.Instructions
                };

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();

                // Add medications
                foreach (var medDto in dto.Medications)
                {
                    var medication = new PrescriptionMedication
                    {
                        PrescriptionId = prescription.PrescriptionId,
                        MedicineName = medDto.MedicineName,
                        Dosage = medDto.Dosage,
                        Frequency = medDto.Frequency,
                        Duration = medDto.Duration,
                        Notes = medDto.Notes
                    };

                    _context.PrescriptionMedications.Add(medication);
                }

                await _context.SaveChangesAsync();

                // Return created prescription
                return await GetPrescriptionByIdAsync(prescription.PrescriptionId)
                    ?? throw new InvalidOperationException("Failed to retrieve created prescription");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription");
                throw;
            }
        }

        public async Task<List<PrescriptionDto>> GetPatientPrescriptionsAsync(int patientId)
        {
            try
            {
                var prescriptions = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Doctor).ThenInclude(d => d.User)
                    .Include(p => p.Medications)
                    .Where(p => p.PatientId == patientId)
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new PrescriptionDto
                    {
                        PrescriptionId = p.PrescriptionId,
                        AppointmentId = p.AppointmentId,
                        PatientId = p.PatientId,
                        PatientName = $"{p.Patient.FirstName} {p.Patient.LastName}",
                        DoctorId = p.DoctorId,
                        DoctorName = $"{p.Doctor.User.FirstName} {p.Doctor.User.LastName}",
                        Diagnosis = p.Diagnosis,
                        Instructions = p.Instructions,
                        CreatedAt = p.CreatedAt,
                        Medications = p.Medications.Select(m => new MedicationDto
                        {
                            MedicationId = m.MedicationId,
                            MedicineName = m.MedicineName,
                            Dosage = m.Dosage,
                            Frequency = m.Frequency,
                            Duration = m.Duration,
                            Notes = m.Notes
                        }).ToList()
                    })
                    .ToListAsync();

                return prescriptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting prescription for patient: {patientId}");
                throw;
            }
        }

        public async Task<PrescriptionDto?> GetPrescriptionByIdAsync(int prescriptionId)
        {
            try
            {
                var prescription = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Doctor).ThenInclude(d => d.User)
                    .Include(p => p.Medications)
                    .Where(p => p.PrescriptionId == prescriptionId)
                    .Select(p => new PrescriptionDto
                    {
                        PrescriptionId = p.PrescriptionId,
                        AppointmentId = p.AppointmentId,
                        PatientId = p.PatientId,
                        PatientName = $"{p.Patient.FirstName} {p.Patient.LastName}",
                        DoctorId = p.DoctorId,
                        DoctorName = $"{p.Doctor.User.FirstName} {p.Doctor.User.LastName}",
                        Diagnosis = p.Diagnosis,
                        Instructions = p.Instructions,
                        CreatedAt = p.CreatedAt,
                        Medications = p.Medications.Select(m => new MedicationDto
                        {
                            MedicationId = m.MedicationId,
                            MedicineName = m.MedicineName,
                            Dosage = m.Dosage,
                            Frequency = m.Frequency,
                            Duration = m.Duration,
                            Notes = m.Notes
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                return prescription; // Now properly returns nullable type
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting prescription by ID: {prescriptionId}");
                throw;
            }
        }

        public async Task<PrescriptionDto?> GetPrescriptionByAppointmentAsync(int appointmentId)
        {
            try
            {
                var prescription = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Doctor).ThenInclude(d => d.User)
                    .Include(p => p.Medications)
                    .Where(p => p.AppointmentId == appointmentId)
                    .Select(p => new PrescriptionDto
                    {
                        PrescriptionId = p.PrescriptionId,
                        AppointmentId = p.AppointmentId,
                        PatientId = p.PatientId,
                        PatientName = $"{p.Patient.FirstName} {p.Patient.LastName}",
                        DoctorId = p.DoctorId,
                        DoctorName = $"{p.Doctor.User.FirstName} {p.Doctor.User.LastName}",
                        Diagnosis = p.Diagnosis,
                        Instructions = p.Instructions,
                        CreatedAt = p.CreatedAt,
                        Medications = p.Medications.Select(m => new MedicationDto
                        {
                            MedicationId = m.MedicationId,
                            MedicineName = m.MedicineName,
                            Dosage = m.Dosage,
                            Frequency = m.Frequency,
                            Duration = m.Duration,
                            Notes = m.Notes
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                return prescription; // Now properly returns nullable type
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting prescription for appointment: {appointmentId}");
                throw;
            }
        }
    }
}