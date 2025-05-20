using AskHire_Backend.Models.Entities;

public interface ICandidateFileRepository
{
    Task<Application?> GetApplicationAsync(Guid applicationId);
    Task<User?> GetUserAsync(Guid userId);
    Task<Vacancy?> GetVacancyAsync(Guid vacancyId);
    Task AddApplicationAsync(Application application);
    Task SaveChangesAsync();
}
