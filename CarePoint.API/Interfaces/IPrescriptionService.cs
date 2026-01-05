using CarePoint.API.DTOs.Prescription;

namespace CarePoint.API.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> CreatePrescriptionAsync(int doctorId, CreatePrescriptionDto dto);
        Task<List<PrescriptionDto>> GetPatientPrescriptionsAsync(int patientId);
        Task<PrescriptionDto?> GetPrescriptionByIdAsync(int prescriptionId);
        Task<PrescriptionDto?> GetPrescriptionByAppointmentAsync(int appointmentId);
    }
}