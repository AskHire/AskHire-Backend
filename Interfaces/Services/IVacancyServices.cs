using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public interface IVacancyService
    {
        Task<Vacancy> CreateVacancyAsync(Vacancy vacancy);
        Task<Vacancy?> GetVacancyByIdAsync(Guid id);
        Task<IEnumerable<Vacancy>> GetAllVacanciesAsync();
        Task<bool> DeleteVacancyAsync(Guid id);
        Task<Vacancy?> UpdateVacancyAsync(Vacancy vacancy);
    }
}