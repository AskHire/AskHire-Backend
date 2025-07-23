using AskHire_Backend.Interfaces.Services.IManagerServices;
using AskHire_Backend.Models.Entities;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.ManagerServices
{
    public class ManagerEmailService : IManagerEmailService
    {
        public async Task<bool> SendInterviewEmailAsync(string recipientEmail, Interview interview)
        {
            try
            {
                if (string.IsNullOrEmpty(recipientEmail))
                {
                    return false;
                }

                var fromAddress = new MailAddress("hello@askhire.com", "ASKHIRE Company");
                var toAddress = new MailAddress(recipientEmail);
                const string subject = "Interview Invitation";

                // Format the duration as hours and minutes
                string durationFormat = interview.Duration.Hours > 0
                    ? $"{interview.Duration.Hours} hour{(interview.Duration.Hours != 1 ? "s" : "")}"
                    : "";

                if (interview.Duration.Minutes > 0)
                {
                    durationFormat += durationFormat.Length > 0 ? " and " : "";
                    durationFormat += $"{interview.Duration.Minutes} minute{(interview.Duration.Minutes != 1 ? "s" : "")}";
                }

                string body = $@"Dear Candidate,

You are invited to an interview scheduled on {interview.Date.ToShortDateString()} at {interview.Time}.
Duration: {durationFormat}
Instructions: {interview.Interview_Instructions}

Best regards,
ASKHIRE Company";

                using (var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("dimashiwickramage2002@gmail.com", "fnxm msjt blvm vnmo"),
                    EnableSsl = true
                })
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    await smtpClient.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }
    }
}