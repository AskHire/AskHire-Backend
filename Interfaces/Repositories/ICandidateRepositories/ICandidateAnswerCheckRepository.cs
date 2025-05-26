using AskHire_Backend.Models.Entities;
using AskHire_Backend.Data.Entities;

namespace AskHire_Backend.Interfaces.Repositories.CandidateRepositories
{
    public interface ICandidateAnswerCheckRepository
    {
        Task<Application?> GetApplicationWithVacancyAsync(Guid applicationId);
        Task<Question?> GetQuestionByIdAsync(Guid questionId);
        Task SaveChangesAsync();
    }
}
