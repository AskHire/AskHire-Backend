using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IVacancyRepository _vacancyRepository;

        public VacancyService(IVacancyRepository vacancyRepository)
        {
            _vacancyRepository = vacancyRepository ?? throw new ArgumentNullException(nameof(vacancyRepository));
        }

        public async Task<Vacancy> CreateVacancyAsync(Vacancy vacancy)
        {
            if (vacancy == null)
            {
                throw new ArgumentNullException(nameof(vacancy), "Vacancy cannot be null.");
            }

            return await _vacancyRepository.CreateVacancyAsync(vacancy);
        }

        public async Task<Vacancy?> GetVacancyByIdAsync(Guid id)
        {
            return await _vacancyRepository.GetVacancyByIdAsync(id);
        }

        public async Task<IEnumerable<Vacancy>> GetAllVacanciesAsync()
        {
            return await _vacancyRepository.GetAllVacanciesAsync();
        }

        public async Task<bool> DeleteVacancyAsync(Guid id)
        {
            return await _vacancyRepository.DeleteVacancyAsync(id);
        }

        public async Task<Vacancy?> UpdateVacancyAsync(Vacancy vacancy)
        {
            if (vacancy == null)
            {
                throw new ArgumentNullException(nameof(vacancy), "Vacancy cannot be null.");
            }

            return await _vacancyRepository.UpdateVacancyAsync(vacancy);
        }


    }
}