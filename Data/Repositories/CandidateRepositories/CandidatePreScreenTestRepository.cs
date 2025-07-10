using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories.CandidateRepositories;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Data.Repositories.CandidateRepositories
{
    public class CandidatePreScreenTestRepository : ICandidatePreScreenTestRepository
    {
        private readonly AppDbContext _context;

        public CandidatePreScreenTestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PreScreenTestDto?> GetVacancyInfoByApplicationId(Guid applicationId)
        {
            var application = await _context.Applies
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null || application.Vacancy == null)
                return null;

            return new PreScreenTestDto
            {
                VacancyName = application.Vacancy.VacancyName,
                QuestionCount = application.Vacancy.QuestionCount,
                Duration = application.Vacancy.Duration
            };
        }

        //public async Task<PreScreenTestDto?> GetQuestionsByApplicationId(Guid applicationId)
        //{
        //    var application = await _context.Applies
        //        .Include(a => a.Vacancy)
        //        .ThenInclude(v => v.JobRole)
        //        .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

        //    if (application == null || application.Vacancy == null || application.Vacancy.JobId == null)
        //        return null;

        //    var jobId = application.Vacancy.JobId.Value;

        //    var questions = await _context.Questions
        //        .Where(q => q.JobId == jobId)
        //        .Take(application.Vacancy.QuestionCount)
        //        .ToListAsync();

        //    var questionDtos = questions.Select(q => new PreScreenQuestionDto
        //    {
        //        QuestionId = q.QuestionId,
        //        QuestionName = q.QuestionName,
        //        Option1 = q.Option1,
        //        Option2 = q.Option2,
        //        Option3 = q.Option3,
        //        Option4 = q.Option4
        //    }).ToList();

        //    return new PreScreenTestDto
        //    {
        //        Questions = questionDtos,
        //        Duration = application.Vacancy.Duration,
        //        QuestionCount = application.Vacancy.QuestionCount,
        //        Status = application.Status
        //    };
        //}

        public async Task<PreScreenTestDto?> GetQuestionsByApplicationId(Guid applicationId)
        {
            var application = await _context.Applies
                .Include(a => a.Vacancy)
                .ThenInclude(v => v.JobRole)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null || application.Vacancy == null || application.Vacancy.JobId == null)
                return null;

            var jobId = application.Vacancy.JobId.Value;
            var questionCount = application.Vacancy.QuestionCount;

            // Step 1: Get all matching question IDs
            var allQuestionIds = await _context.Questions
                .Where(q => q.JobId == jobId)
                .Select(q => q.QuestionId)
                .ToListAsync();

            // Step 2: Randomly pick questionCount IDs in memory
            var random = new Random();
            var randomQuestionIds = allQuestionIds
                .OrderBy(x => random.Next())
                .Take(questionCount)
                .ToList();

            // Step 3: Fetch full question details for selected IDs
            var questions = await _context.Questions
                .Where(q => randomQuestionIds.Contains(q.QuestionId))
                .ToListAsync();

            var questionDtos = questions.Select(q => new PreScreenQuestionDto
            {
                QuestionId = q.QuestionId,
                QuestionName = q.QuestionName,
                Option1 = q.Option1,
                Option2 = q.Option2,
                Option3 = q.Option3,
                Option4 = q.Option4
            }).ToList();

            return new PreScreenTestDto
            {
                Questions = questionDtos,
                Duration = application.Vacancy.Duration,
                QuestionCount = application.Vacancy.QuestionCount,
                Status = application.Status
            };
        }

    }
}