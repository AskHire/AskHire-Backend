using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            DashboardStatsDto stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
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
