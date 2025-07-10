using AskHire_Backend.Models.DTOs.ManagerDTOs;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories
{
    public interface IApplicationRepository
    {
        Task<InterviewStatusSummaryDto> GetInterviewStatusSummaryAsync();
    }
}
