namespace CarePoint.API.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendAppointmentConfirmationAsync(string toEmail, string patientName, string doctorName, DateTime appointmentDate, TimeSpan appointmentTime);
        Task SendAppointmentReminderAsync(string toEmail, string patientName, string doctorName, DateTime appointmentDate, TimeSpan appointmentTime);
    }
}