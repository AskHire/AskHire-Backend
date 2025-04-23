using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Change the return type to Application if Apply doesn't exist
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

        public async Task<Interview> UpdateInterviewAsync(Interview interview)
        {
            _context.Interviews.Update(interview);
            await _context.SaveChangesAsync();
            return interview;
        }

        public async Task<Interview> GetInterviewByApplicationIdAsync(Guid applicationId)
        {
            return await _context.Interviews
                .FirstOrDefaultAsync(i => i.ApplicationId == applicationId);
        }

        public async Task<List<Interview>> GetAllInterviewsAsync()
        {
            return await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(a => a.User)
                .ToListAsync();
        }

        public async Task<List<Models.DTOs.UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId)
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
                    Instructions = i.Instructions
                })
                .ToListAsync();
        }

    }
}

