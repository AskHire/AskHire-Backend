using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalCandidates = await _context.Users.CountAsync(u => u.Role == "Candidate");
            var totalManagers = await _context.Users.CountAsync(u => u.Role == "Manager");
            var totalJobs = await _context.JobRoles.CountAsync();

            return Ok(new
            {
                totalUsers,
                totalCandidates,
                totalManagers,
                totalJobs
            });
        }
    }
}
