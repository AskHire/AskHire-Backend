using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Data.Repositories
{
    public class VacancyRepository : IVacancyRepository
    {
        private readonly AppDbContext _context;

        public VacancyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Vacancy> CreateVacancyAsync(Vacancy vacancy)
        {
            _context.Vacancies.Add(vacancy);
            await _context.SaveChangesAsync();
            return vacancy;
        }

        public async Task<Vacancy?> GetVacancyByIdAsync(Guid id)
        {
            return await _context.Vacancies.Include(v => v.JobRole)
                .FirstOrDefaultAsync(v => v.VacancyId == id);
        }

        public async Task<IEnumerable<Vacancy>> GetAllVacanciesAsync()
        {
            return await _context.Vacancies.Include(v => v.JobRole).ToListAsync();
        }

        public async Task<bool> DeleteVacancyAsync(Guid id)
        {
            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return false;
            }

            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Vacancy?> UpdateVacancyAsync(Vacancy vacancy)
        {
            var existingVacancy = await _context.Vacancies.FindAsync(vacancy.VacancyId);
            if (existingVacancy == null)
            {
                return null;
            }

            _context.Entry(existingVacancy).CurrentValues.SetValues(vacancy);
            await _context.SaveChangesAsync();
            return existingVacancy;
        }


        //eshan
        public async Task<IEnumerable<JobWiseVacancyDto>> GetJobWiseVacanciesAsync()
        {
            return await _context.Vacancies
                .Include(v => v.JobRole)
                .Select(v => new JobWiseVacancyDto
                {
                    VacancyId = v.VacancyId,
                    VacancyName = v.VacancyName,
                    WorkType = v.JobRole != null ? v.JobRole.WorkType : "N/A",
                    WorkLocation = v.JobRole != null ? v.JobRole.WorkLocation : "N/A",
                    Description = v.JobRole != null ? v.JobRole.Description : "N/A",
                    Instructions = v.Instructions,
                    EndDate = v.EndDate
                })
                .ToListAsync();
        }

    }
}