using AskHire_Backend.Models.Entities;
using AskHire_Backend.Services.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendInterviewEmailAsync(string recipientEmail, Interview interview)
        {
            try
            {
                if (string.IsNullOrEmpty(recipientEmail))
                {
                    return false;
                }

                var fromAddress = new MailAddress("youremail@example.com", "Your Company");
                var toAddress = new MailAddress(recipientEmail);
                const string subject = "Interview Invitation";
                string body = $@"Dear Candidate,
You are invited to an interview scheduled on {interview.Date.ToShortDateString()} at {interview.Time}.
Instructions: {interview.Instructions}
Best regards,
Your Company";

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