using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Repositories
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly AppDbContext _context;

        public InterviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Application> GetApplicationWithUserAsync(Guid applicationId)
        {
            return await _context.Applies
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }

        public async Task<Interview> CreateInterviewAsync(Interview interview)
        {
            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();
            return interview;
        }

        public Task<Application> GetApplicationWithUserAsync(int applicationId)
        {
            throw new NotImplementedException();
        }
    }
}