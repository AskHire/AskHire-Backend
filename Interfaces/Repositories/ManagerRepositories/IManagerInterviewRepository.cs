using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Repositories.Interfaces
{
    public interface IManagerInterviewRepository
    {
        Task<Application> GetApplicationWithUserAsync(Guid applicationId);
        Task<Interview> CreateInterviewAsync(Interview interview);
        Task<Interview> UpdateInterviewAsync(Interview interview);
        Task<Interview> GetInterviewByApplicationIdAsync(Guid applicationId);
        Task<List<Interview>> GetAllInterviewsAsync();
        Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId);
    }
}