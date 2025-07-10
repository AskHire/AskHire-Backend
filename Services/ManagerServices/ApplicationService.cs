using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Models.DTOs.ManagerDTOs;

namespace AskHire_Backend.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<InterviewStatusSummaryDto> GetInterviewStatusSummaryAsync()
        {
            return await _applicationRepository.GetInterviewStatusSummaryAsync();
        }
    }
}
