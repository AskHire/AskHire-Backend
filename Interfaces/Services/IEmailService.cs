using AskHire_Backend.Models.Entities;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendInterviewEmailAsync(string recipientEmail, Interview interview);
    }
}