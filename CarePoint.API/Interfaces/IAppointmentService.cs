using CarePoint.API.DTOs.Appointment;

namespace CarePoint.API.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto);
        Task<List<AppointmentDto>> GetPatientAppointmentsAsync(int patientId);
        Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId);
        Task<AppointmentDto?> GetAppointmentByIdAsync(int appointmentId);
        Task<AppointmentDto> UpdateAppointmentAsync(int appointmentId, UpdateAppointmentDto dto);
        Task<bool> CancelAppointmentAsync(int appointmentId, int userId, CancelAppointmentDto dto);
        Task<bool> CompleteAppointmentAsync(int appointmentId, CompleteAppointmentDto dto);
    }
}