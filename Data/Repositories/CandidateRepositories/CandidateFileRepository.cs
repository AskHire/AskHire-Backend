using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class CandidateFileRepository : ICandidateFileRepository
{
    private readonly AppDbContext _context;

    public CandidateFileRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetUserAsync(Guid userId) =>
        _context.Users.FindAsync(userId).AsTask();

    public Task<Vacancy?> GetVacancyAsync(Guid vacancyId) =>
        _context.Vacancies.FindAsync(vacancyId).AsTask();

    public Task<Application?> GetApplicationAsync(Guid applicationId) =>
        _context.Applies.FindAsync(applicationId).AsTask();

    public async Task AddApplicationAsync(Application application)
    {
        await _context.Applies.AddAsync(application);
    }

    public async Task<Application?> GetApplicationWithVacancyAsync(Guid applicationId)
    {
        return await _context.Applies
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
    }


    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
