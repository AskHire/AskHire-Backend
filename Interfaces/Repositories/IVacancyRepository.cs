using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories
{
    public interface IVacancyRepository
    {
        Task<Vacancy> CreateVacancyAsync(Vacancy vacancy);
        Task<Vacancy?> GetVacancyByIdAsync(Guid id);
        Task<IEnumerable<Vacancy>> GetAllVacanciesAsync();
        Task<bool> DeleteVacancyAsync(Guid id);
        Task<Vacancy?> UpdateVacancyAsync(Vacancy vacancy);
    }
}