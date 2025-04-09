using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        Task<Application> GetApplicationWithUserAsync(Guid applicationId);
        Task<Interview> CreateInterviewAsync(Interview interview);
        Task<Application> GetApplicationWithUserAsync(int applicationId);
    }
}