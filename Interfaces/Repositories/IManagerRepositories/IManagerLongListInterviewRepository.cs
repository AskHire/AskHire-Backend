using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories.IManagerRepositories
{
    public interface IManagerLongListInterviewRepository
    {
        Task<Vacancy> GetVacancyByNameAsync(string vacancyName);
        Task<Vacancy> GetVacancyByIdAsync(Guid vacancyId);
        Task<IEnumerable<Application>> GetUnscheduledApplicationsAsync(Guid vacancyId);
        Task<IEnumerable<Application>> GetLongListApplicationsAsync(Guid vacancyId);
        Task<bool> HasExistingInterviewAsync(Guid applicationId);
        Task<bool> SaveInterviewAsync(Interview interview);
        Task<IEnumerable<Interview>> GetScheduledInterviewsAsync(Guid vacancyId);

        Task<bool> UpdateApplicationAsync(Application application);
    }
}