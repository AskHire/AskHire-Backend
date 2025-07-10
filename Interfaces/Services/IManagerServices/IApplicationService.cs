using AskHire_Backend.Models.DTOs.ManagerDTOs;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<InterviewStatusSummaryDto> GetInterviewStatusSummaryAsync();
    }
}
