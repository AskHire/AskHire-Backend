using AskHire_Backend.Models.Entities;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services.IManagerServices
{
    public interface IManagerEmailService
    {
        Task<bool> SendInterviewEmailAsync(string recipientEmail, Interview interview);
        
    }
}