using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using AskHire_Backend.Interfaces.Services; // For InvalidOperationException, ApplicationException

namespace AskHire_Backend.Services
{
    public class AuthEmailService : IAuthEmailService // This is your new service name
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthEmailService> _logger;

        public AuthEmailService(IConfiguration configuration, ILogger<AuthEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // Retrieve SMTP settings from configuration
            var smtpHost = _configuration["SmtpSettings:Host"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"] ?? "587"); // Default to 587 if not found
            var smtpUser = _configuration["SmtpSettings:Username"];
            var smtpPass = _configuration["SmtpSettings:Password"];
            var fromEmail = _configuration["SmtpSettings:FromEmail"];

            // Validate if essential SMTP settings are configured
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass) || string.IsNullOrEmpty(fromEmail))
            {
                _logger.LogError("SMTP settings are incomplete or missing. Cannot send email.");
                throw new InvalidOperationException("SMTP settings are incomplete. Please ensure Host, Port, Username, Password, and FromEmail are configured in appsettings.json.");
            }

            // Create and configure SmtpClient
            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true; // Most SMTP servers require SSL/TLS
                client.UseDefaultCredentials = false; // Important for external SMTP servers
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network; // Ensures it uses the network for sending

                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "AskHire Support"), // Sender's email and display name
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true // Set to true if your email body contains HTML
                };
                mailMessage.To.Add(toEmail); // Add recipient

                try
                {
                    // Send the email
                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully to {ToEmail} for subject '{Subject}'.", toEmail, subject);
                }
                catch (SmtpException smtpEx)
                {
                    // Log specific SMTP errors
                    _logger.LogError(smtpEx, "SMTP error sending email to {ToEmail} for subject '{Subject}'. SMTP Status Code: {StatusCode}. Message: {SmtpErrorMessage}", toEmail, subject, smtpEx.StatusCode, smtpEx.Message);
                    throw new ApplicationException($"SMTP error sending email: {smtpEx.Message}", smtpEx);
                }
                catch (Exception ex)
                {
                    // Log general email sending failures
                    _logger.LogError(ex, "Failed to send email to {ToEmail} for subject '{Subject}'. Message: {ErrorMessage}", toEmail, subject, ex.Message);
                    throw new ApplicationException($"Error sending email: {ex.Message}", ex);
                }
            }
        }
    }
}