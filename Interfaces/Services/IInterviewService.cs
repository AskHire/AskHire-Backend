using AskHire_Backend.Models.DTOs;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.Interfaces
{
    public interface IInterviewService
    {
        Task<bool> ScheduleInterviewAsync(InterviewScheduleRequest interviewRequest);
    }
}