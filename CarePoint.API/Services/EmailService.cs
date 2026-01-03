using MailKit.Net.Smtp;
using MaileKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using CarePoint.API.Configuration;
using CarePoint.API.Interfaces;

namespace CarePoint.API.services
{
    // Service for sending email
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILoger _logger;

        public EmailService(IOptions emailSettings, ILogger logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Failed to send email to { toEmail }");
                throw;
            }
        }

        public async Task SendAppointmentConfirmationAsync(string toEmail, string patientName, string doctorName, DateTime appointmentDate, TimeSpan appointmentTime)
        {
            var subject = "Appointment Confirmation - CarePoint";
            var body = $@"
            
            
                    Appointment Confirmed
                    Dear { patientName },
                    Your appointment has been successfully scheduled:

                        Doctor: Dr. { doctorName }
                        Date: { appointmentDate:MMMM dd, yyyy }
                        Time: { appointmentTime: hh\\:mm }

                    Please arrive 15 minutes before your scheduled time.
                    Best regards, CarePoint Healthcare Team

                ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAppointmentRemiderAsync(string toEmail, string patientName, DateTime appointmentDate, TimeSopan appointmentTime)
        {
            var subject = "Appointment Reminder - Tomorrow";
            var body = $@"
            
            
                    Appointment Reminder
                    Dear { patientName },
                    This is a friendly reminder about your upcoming appointment:

                        Doctor: Dr. { doctorName }
                        Date: { appointmentDate:MMMM dd, yyyy }
                        Time: { appointmentTime:hh\\:mm }
                    
                    If you need to cancel or reschedule, please contact us as soon as possible.
                    Best regards, CarePoint Healthcare Team

                ";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}