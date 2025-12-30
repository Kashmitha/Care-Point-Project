namespace CarePoint.API.Configuration
{
    public class EmailSettings
    {
        // SMTP server address for sending emails.
        public string SmtpServer { get; set; } = string.Empty;

        // SMTP server port.
        public int SmtpPort { get; set; }

        // Sender email address.
        public string SenderEmail { get; set; } = string.Empty;

        // Sender display name.
        public string SenderName { get; set; } = string.Empty;

        // SMTP server username.
        public string Username { get; set; } = string.Empty;

        // SMTP server password.
        public string Password { get; set; } = string.Empty;
    }
}