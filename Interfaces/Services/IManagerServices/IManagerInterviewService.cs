using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.Interfaces
{
    public interface IManagerInterviewService
    {
        Task<bool> ScheduleInterviewAsync(ManagerInterviewScheduleRequestDTO interviewRequest);
        Task<bool> UpdateInterviewAsync(Guid interviewId, ManagerInterviewScheduleRequestDTO interviewRequest);
        Task<Interview> GetInterviewByApplicationIdAsync(Guid applicationId);
        Task<List<Interview>> GetAllInterviewsAsync();
        Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId);
        

    }
}