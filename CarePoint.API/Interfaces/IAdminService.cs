using CarePoint.API.DTOs.Admin;

namespace CarePoint.API.Interfaces
{
    public interface IAdminService
    {
        Task<AdminStatsDto> GetDashboardStatsAsync();
        Task<List<DoctorApprovalDto>> GetPendingDoctorsAsync();
        Task<bool> ApproveDoctorAsync(int doctorId, int adminId);
        Task<bool> RejectDoctorAsync(int doctorId);
    }
}