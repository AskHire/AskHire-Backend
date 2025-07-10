using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories.CandidateRepositories;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Data.Repositories.CandidateRepositories
{
    public class CandidateAnswerCheckRepository : ICandidateAnswerCheckRepository
    {
        private readonly AppDbContext _context;

        public CandidateAnswerCheckRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Application?> GetApplicationWithVacancyAsync(Guid applicationId)
        {
            return await _context.Applies
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }

        public async Task<Question?> GetQuestionByIdAsync(Guid questionId)
        {
            return await _context.Questions
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PreScreenPassMarkDto> GetPreScreenPassMarkAndEmailAsync(Guid applicationId)
        {
            var application = await _context.Applies
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null)
                throw new Exception("Application not found");

            return new PreScreenPassMarkDto
            {
                Email = application.User.Email,
                PreScreenPassMark = application.Pre_Screen_PassMark
            };
        }

    }
}
