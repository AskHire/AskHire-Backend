using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories.CandidateRepositories;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Data.Repositories.CandidateRepositories
{
    public class CandidateInterviewRepository : ICandidateInterviewRepository
    {
        private readonly AppDbContext _context;

        public CandidateInterviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId)
        {
            return await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(a => a.Vacancy)
                .Where(i => i.Application.UserId == userId)
                .Select(i => new UserInterviewDetailsDto
                {
                    UserId = i.Application.UserId,
                    ApplicationId = i.ApplicationId,
                    VacancyId = i.Application.Vacancy.VacancyId,
                    VacancyName = i.Application.Vacancy.VacancyName,
                    InterviewDate = i.Date,
                    InterviewTime = i.Time,
                    InterviewInstructions = i.Interview_Instructions
                })
                .ToListAsync();
        }
    }
}