using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Repositories;
using Microsoft.EntityFrameworkCore;


namespace AskHire_Backend.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.JobRole)
                .ToListAsync();
        }

        public async Task<Question?> GetQuestionByIdAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.JobRole)
                .FirstOrDefaultAsync(q => q.QuestionId == id);
        }

        public async Task<Question> CreateQuestionAsync(QuestionDTO questionDTO)
        {
            var jobRole = await _context.JobRoles.FindAsync(questionDTO.JobId);
            if (jobRole == null)
            {
                throw new ArgumentException($"JobRole with ID {questionDTO.JobId} not found.");
            }

            var question = new Question
            {
                JobId = questionDTO.JobId,
                JobRole = jobRole,
                QuestionName = questionDTO.QuestionName,
                Option1 = questionDTO.Option1,
                Option2 = questionDTO.Option2,
                Option3 = questionDTO.Option3,
                Option4 = questionDTO.Option4,
                Answer = questionDTO.Answer
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateQuestionAsync(Guid id, QuestionDTO questionDTO)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                throw new KeyNotFoundException($"Question with ID {id} not found.");
            }

            // Check if JobId was changed and update if necessary
            if (question.JobId != questionDTO.JobId)
            {
                var jobRole = await _context.JobRoles.FindAsync(questionDTO.JobId);
                if (jobRole == null)
                {
                    throw new ArgumentException($"JobRole with ID {questionDTO.JobId} not found.");
                }
                question.JobId = questionDTO.JobId;
                question.JobRole = jobRole;
            }

            question.QuestionName = questionDTO.QuestionName;
            question.Option1 = questionDTO.Option1;
            question.Option2 = questionDTO.Option2;
            question.Option3 = questionDTO.Option3;
            question.Option4 = questionDTO.Option4;
            question.Answer = questionDTO.Answer;

            _context.Questions.Update(question);
            await _context.SaveChangesAsync();

            return question;
        }

        public async Task DeleteQuestionAsync(Guid id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                throw new KeyNotFoundException($"Question with ID {id} not found.");
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }
    }
}