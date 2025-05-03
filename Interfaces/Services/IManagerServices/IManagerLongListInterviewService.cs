using AskHire_Backend.Models.DTOs.ManagerDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services.IManagerServices
{
    public interface IManagerLongListInterviewService
    {
        Task<LongListInterviewResultDTO> ScheduleLongListInterviewsAsync(ManagerLongListInterviewScheduleRequestDTO request);
        Task<List<UnscheduledCandidateDTO>> GetUnscheduledCandidatesAsync(Guid vacancyId);
        Task<List<ScheduledInterviewDTO>> GetScheduledInterviewsAsync(Guid vacancyId);
        

    }
}