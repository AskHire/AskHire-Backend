using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Models.Entities;

public interface ICandidateFileRepository
{
    Task<Application?> GetApplicationAsync(Guid applicationId);
    Task<User?> GetUserAsync(Guid userId);
    Task<Vacancy?> GetVacancyAsync(Guid vacancyId);
    Task AddApplicationAsync(Application application);
    Task SaveChangesAsync();
    Task<Application?> GetApplicationWithVacancyAsync(Guid applicationId);
    //Task<int?> GetCVMarkByApplicationIdAsync(Guid applicationId);
    Task<ApplicationCVStatusDto?> GetCVMarkAndStatusAsync(Guid applicationId);
    Task<ApplicationCVMarkDto?> GetCVMarkAndEmailAsync(Guid applicationId);
    Task<Application?> GetByUserAndVacancyAsync(Guid userId, Guid vacancyId);

}
