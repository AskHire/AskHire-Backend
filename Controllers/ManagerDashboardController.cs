using AskHire_Backend.Models.Entities;
using AskHire_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/manager-dashboard")]
    [ApiController]
    public class ManagerDashboardController : ControllerBase
    {
        private readonly Interfaces.Services.IManagerDashboardService _ManagerService;
        private readonly ILogger<ManagerDashboardController> _logger;

        public ManagerDashboardController(Interfaces.Services.IManagerDashboardService userService, ILogger<ManagerDashboardController> logger)
        {
            _ManagerService = userService;
            _logger = logger;
        }

        [HttpGet("total-candidates")]
        public async Task<ActionResult<int>> GetTotalUsers()
        {
            try
            {
                var count = await _ManagerService.GetTotalUsersAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching total candidates count.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("total-jobs")]
        public async Task<IActionResult> GetTotalJobs()
        {
            try
            {
                var count = await _ManagerService.GetTotalJobsAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching total jobs count.");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}