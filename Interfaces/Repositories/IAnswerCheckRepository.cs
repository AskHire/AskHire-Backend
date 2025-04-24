using AskHire_Backend.Models.Entities;
using AskHire_Backend.Data.Entities;

namespace AskHire_Backend.Repositories.Interfaces
{
    public interface IAnswerCheckRepository
    {
        Task<Application?> GetApplicationWithVacancyAsync(Guid applicationId);
        Task<Question?> GetQuestionByIdAsync(Guid questionId);
        Task SaveChangesAsync();
    }
}
