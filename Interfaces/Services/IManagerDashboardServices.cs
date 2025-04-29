using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services
{
    public interface IManagerDashboardService
    {
        // Nullable return type
        Task<int> GetTotalJobsAsync();
        Task<int> GetTotalUsersAsync();
    }
}
