using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories
{
    public interface IManagerLonglistIVacancyRepository
    {
        Task<IEnumerable<Vacancy>> GetAllVacanciesAsync();
        Task<Vacancy?> GetVacancyByIdAsync(Guid id);
        Task<Vacancy> CreateVacancyAsync(Vacancy vacancy);
        Task<Vacancy?> UpdateVacancyAsync(Vacancy vacancy);
        Task<bool> DeleteVacancyAsync(Guid id);
        Task<Vacancy?> GetVacancyByJobIdAsync(Guid jobId);
    }
}