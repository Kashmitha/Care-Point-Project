using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using CarePoint.API.Configuration;
using CarePoint.API.Interfaces;

namespace CarePoint.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }
        }

        public async Task SendAppointmentConfirmationAsync(string toEmail, string patientName, string doctorName, DateTime appointmentDate, TimeSpan appointmentTime)
        {
            var subject = "Appointment Confirmation - CarePoint";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2563eb;'>Appointment Confirmed</h2>
                    <p>Dear {patientName},</p>
                    <p>Your appointment has been successfully scheduled:</p>
                    <div style='background-color: #f3f4f6; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Doctor:</strong> Dr. {doctorName}</p>
                        <p><strong>Date:</strong> {appointmentDate:MMMM dd, yyyy}</p>
                        <p><strong>Time:</strong> {appointmentTime:hh\\:mm}</p>
                    </div>
                    <p>Please arrive 15 minutes before your scheduled time.</p>
                    <p>Best regards,<br/>CarePoint Healthcare Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
        
        public async Task SendAppointmentReminderAsync(string toEmail, string patientName, string doctorName, DateTime appointmentDate, TimeSpan appointmentTime)
        {
            var subject = "Appointment Reminder - Tomorrow";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2563eb;'>Appointment Reminder</h2>
                    <p>Dear {patientName},</p>
                    <p>This is a friendly reminder about your upcoming appointment:</p>
                    <div style='background-color: #fef3c7; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Doctor:</strong> Dr. {doctorName}</p>
                        <p><strong>Date:</strong> {appointmentDate:MMMM dd, yyyy}</p>
                        <p><strong>Time:</strong> {appointmentTime:hh\\:mm}</p>
                    </div>
                    <p>If you need to cancel or reschedule, please contact us as soon as possible.</p>
                    <p>Best regards,<br/>CarePoint Healthcare Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}