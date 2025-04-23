using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.Interfaces
{
    public interface IInterviewService
    {
        Task<bool> ScheduleInterviewAsync(InterviewScheduleRequestDTO interviewRequest);
        Task<bool> UpdateInterviewAsync(Guid interviewId, InterviewScheduleRequestDTO interviewRequest);
        Task<Interview> GetInterviewByApplicationIdAsync(Guid applicationId);
        Task<List<Interview>> GetAllInterviewsAsync();
    }
}