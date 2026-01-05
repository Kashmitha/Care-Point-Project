using CarePoint.API.DTOs.Doctor;

namespace CarePoint.API.Interfaces
{
    /// <summary>
    /// Interface for doctor related operations
    /// </summary>
    public interface IDoctorService
    {
        Task<List<DoctorProfileDto>> SearchDoctorAsync(DoctorSearchDto serachDto);
        Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId);
        Task<List<DoctorAvailabilityDto>> GetDoctorAvailabilityAsync(int doctorId);
        Task<DoctorAvailabilityDto> AddAvailabilityAync(int doctorId, CreateAvailabilityDto dto);
        Task<bool> DeleteAvailabilityAsync(int doctorId, int availabilityId);
        Task<DoctorProfileDto> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto dto);
    }
}