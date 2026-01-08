using Microsoft.EntityFrameworkCore;
using CarePoint.API.Data;
using CarePoint.API.DTOs.Doctor;
using CarePoint.API.Interfaces;
using CarePoint.API.Models;

namespace CarePoint.API.Services
{
    ///
    /// Service for doctor related operations
    ///
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorService> _logger;
        public DoctorService(ApplicationDbContext context, ILogger<DoctorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<DoctorProfileDto>> SearchDoctorsAsync(DoctorSearchDto searchDto)
        {
            try
            {
                var pageNumber = searchDto.PageNumber < 1 ? 1 : searchDto.PageNumber;
                var pageSize = searchDto.PageSize < 1 ? 10 : searchDto.PageSize;

                // Starts with approved doctors only
                var query = _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Specialty)
                    .Include(d => d.Reviews)
                    .AsNoTracking()
                    .Where(d => d.IsApproved);

                // Apply filters
                if (!string.IsNullOrEmpty(searchDto.SearchTerm))
                {
                    var term = searchDto.SearchTerm.Trim();
                    query = query.Where(d =>
                        d.User.FirstName.Contains(term) ||
                        d.User.LastName.Contains(term) ||
                        (d.Specialty != null && d.Specialty.SpecialtyName.Contains(term)));
                }

                if (searchDto.SpecialtyId.HasValue)
                {
                    query = query.Where(d => d.SpecialtyId == searchDto.SpecialtyId);
                }

                if (!string.IsNullOrEmpty(searchDto.City))
                {
                    query = query.Where(d => d.User.City == searchDto.City);
                }

                if (searchDto.MaxFee.HasValue)
                {
                    query = query.Where(d => d.ConsultationFee <= searchDto.MaxFee);
                }

                // Apply pagination 
                var doctors = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(d => new DoctorProfileDto
                    {
                        DoctorId = d.DoctorId,
                        UserId = d.UserId,
                        FirstName = d.User.FirstName,
                        LastName = d.User.LastName,
                        Email = d.User.Email,
                        PhoneNumber = d.User.PhoneNumber,
                        SpecialtyName = d.Specialty != null ? d.Specialty.SpecialtyName : null,
                        SpecialtyId = d.SpecialtyId,
                        LicenseNumber = d.LicenseNumber,
                        YearsOfExperience = d.YearsOfExperience,
                        Qualification = d.Qualification,
                        Bio = d.Bio,
                        ConsultationFee = d.ConsultationFee,
                        IsApproved = d.IsApproved,
                        AverageRating = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0,
                        TotalReviews = d.Reviews.Count
                    })
                    .ToListAsync();

                return doctors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors");
                throw;
            }
        }

        public async Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Specialty)
                    .Include(d => d.Reviews)
                    .AsNoTracking()
                    .Where(d => d.DoctorId == doctorId)
                    .Select(d => new DoctorProfileDto
                    {
                        DoctorId = d.DoctorId,
                        UserId = d.UserId,
                        FirstName = d.User.FirstName,
                        LastName = d.User.LastName,
                        Email = d.User.Email,
                        PhoneNumber = d.User.PhoneNumber,
                        SpecialtyName = d.Specialty != null ? d.Specialty.SpecialtyName : null,
                        SpecialtyId = d.SpecialtyId,
                        LicenseNumber = d.LicenseNumber,
                        YearsOfExperience = d.YearsOfExperience,
                        Qualification = d.Qualification,
                        Bio = d.Bio,
                        ConsultationFee = d.ConsultationFee,
                        IsApproved = d.IsApproved,
                        AverageRating = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0,
                        TotalReviews = d.Reviews.Count
                    })
                    .FirstOrDefaultAsync();

                return doctor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctor profile for ID: {doctorId}");
                throw;
            }
        }

        public async Task<List<DoctorAvailabilityDto>> GetDoctorAvailabilityAsync(int doctorId)
        {
            try
            {
                var availabilities = await _context.DoctorAvailabilities
                    .Where(da => da.DoctorId == doctorId && da.IsActive)
                    .OrderBy(da => da.DayOfWeek)
                    .ThenBy(da => da.StartTime)
                    .Select(da => new DoctorAvailabilityDto
                    {
                        AvailabilityId = da.AvailabilityId,
                        DoctorId = da.DoctorId,
                        DayOfWeek = da.DayOfWeek,
                        DayName = GetDayName(da.DayOfWeek),
                        StartTime = da.StartTime,
                        EndTime = da.EndTime,
                        IsActive = da.IsActive
                    })
                    .ToListAsync();

                return availabilities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting availability for doctor ID: {doctorId}");
                throw;
            }
        }

        public async Task<DoctorAvailabilityDto> AddAvailabilityAsync(int doctorId, CreateAvailabilityDto dto)
        {
            try
            {
                // Validate doctor exists
                var doctorExists = await _context.Doctors.AnyAsync(d => d.DoctorId == doctorId);
                if (!doctorExists)
                {
                    throw new InvalidOperationException("Doctor not found");
                }

                if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6)
                {
                    throw new InvalidOperationException("Day of week must be between 0 and 6");
                }

                // Validate time range
                if (dto.StartTime >= dto.EndTime)
                {
                    throw new InvalidOperationException("Start time must be before end time");
                }

                // Check for overlapping availability - To avoid double bookings, to maintain data integrity
                var hasOverlap = await _context.DoctorAvailabilities
                    .AnyAsync(da =>
                        da.DoctorId == doctorId &&
                        da.DayOfWeek == dto.DayOfWeek &&
                        da.IsActive &&
                        ((dto.StartTime >= da.StartTime && dto.StartTime < da.EndTime) ||
                        (dto.EndTime > da.StartTime && dto.EndTime <= da.EndTime) ||
                        (dto.StartTime <= da.StartTime && dto.EndTime >= da.EndTime)));

                if (hasOverlap)
                {
                    throw new InvalidOperationException("This availability overlaps with existing schedule");
                }

                var availability = new DoctorAvailability
                {
                    DoctorId = doctorId,
                    DayOfWeek = dto.DayOfWeek,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsActive = true
                };

                _context.DoctorAvailabilities.Add(availability);
                await _context.SaveChangesAsync();

                return new DoctorAvailabilityDto
                {
                    AvailabilityId = availability.AvailabilityId,
                    DoctorId = availability.DoctorId,
                    DayOfWeek = availability.DayOfWeek,
                    DayName = GetDayName(availability.DayOfWeek),
                    StartTime = availability.StartTime,
                    EndTime = availability.EndTime,
                    IsActive = availability.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding availability for doctor ID: {doctorId}");
                throw;
            }
        }

        public async Task<bool> DeleteAvailabilityAsync(int doctorId, int availabilityId)
        {
            try
            {
                var availability = await _context.DoctorAvailabilities
                    .FirstOrDefaultAsync(da => da.AvailabilityId == availabilityId && da.DoctorId == doctorId);

                if (availability == null)
                {
                    return false;
                }

                _context.DoctorAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting availability ID: {availabilityId}");
                throw;
            }
        }

        public async Task<DoctorProfileDto> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto dto)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

                if (doctor == null)
                {
                    throw new InvalidOperationException("Doctor not found");
                }

                // Update doctor fields
                doctor.Bio = dto.Bio;
                doctor.ConsultationFee = dto.ConsultationFee;
                doctor.YearsOfExperience = dto.YearsOfExperience;
                doctor.Qualification = dto.Qualification;

                // Update user fields
                doctor.User.PhoneNumber = dto.PhoneNumber;
                doctor.User.Address = dto.Address;
                doctor.User.City = dto.City;
                doctor.User.State = dto.State;
                doctor.User.ZipCode = dto.ZipCode;
                doctor.User.UpdatedAt = DateTime.UtcNow;
                doctor.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return await GetDoctorProfileAsync(doctorId) ?? throw new InvalidOperationException("Failed to retrieve updated profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating doctor profile ID: {doctorId}");
                throw;
            }
        }

        // Helper method to get day name from day number
        private static string GetDayName(int dayOfWeek)
        {
            return dayOfWeek switch
            {
                0 => "Sunday",
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                6 => "Saturday",
                _ => "Unknown"
            };
        }
    }
}