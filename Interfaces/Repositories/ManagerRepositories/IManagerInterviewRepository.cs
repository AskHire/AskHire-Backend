using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories.ManagerRepositories
{
    public interface IManagerInterviewRepository
    {
        // ... other method signatures ...
        Task<Application> GetApplicationWithUserAsync(Guid applicationId);
        Task<Interview> CreateInterviewAsync(Interview interview);
        Task<Interview> UpdateInterviewAsync(Interview interview);
        Task<Interview> GetInterviewByApplicationIdAsync(Guid applicationId);
        Task<List<Interview>> GetAllInterviewsAsync();
        Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId);
        Task UpdateApplicationAsync(Application application);
    }
} 