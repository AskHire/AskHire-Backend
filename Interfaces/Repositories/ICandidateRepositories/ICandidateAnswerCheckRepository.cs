using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Interfaces.Repositories.CandidateRepositories
{
    public interface ICandidateAnswerCheckRepository
    {
        Task<Application?> GetApplicationWithVacancyAsync(Guid applicationId);
        Task<Question?> GetQuestionByIdAsync(Guid questionId);
        Task SaveChangesAsync();
        Task<PreScreenPassMarkDto> GetPreScreenPassMarkAndEmailAsync(Guid applicationId);
    }
}
