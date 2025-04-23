// Extensions/ServiceRegistration.cs
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Repositories;
using AskHire_Backend.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AskHire_Backend.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<ICandidateRepository, CandidateRepository>();

            // Register services
            services.AddScoped<ICandidateService, CandidateService>();

            // Add other repositories and services here
        }
    }
}