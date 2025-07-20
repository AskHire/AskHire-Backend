namespace AskHire_Backend.Interfaces.Services
{
    public interface IAuthEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
