
﻿using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Services;
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

        // GET api/admindashboard/vacancy-tracking
        [HttpGet("vacancy-tracking")]
        public async Task<IActionResult> GetVacancyTracking([FromQuery] PaginationQuery query)
        {
            var result = await _dashboardService.GetPagedVacancyTrackingAsync(query);
            return Ok(result);
        }
    }
}
