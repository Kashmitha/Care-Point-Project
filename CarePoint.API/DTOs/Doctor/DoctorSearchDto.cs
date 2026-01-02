namespace CarePoint.API.DTOs.Doctor
{
    public class DoctorSearchDto
    {
        public string? SearchTerm { get; set; }
        public int? SpecialtyId { get; set; }
        public string? City { get; set; }
        public decimal? MaxFee { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}