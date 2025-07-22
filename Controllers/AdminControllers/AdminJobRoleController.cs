using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers.AdminControllers
{
    [Route("api/adminjobrole")]
    [ApiController]
    public class AdminJobRoleController : ControllerBase
    {
        private readonly IAdminJobRoleService _service;

        public AdminJobRoleController(IAdminJobRoleService service)
        {
            _service = service;
        }

        // ✅ Pagination, search & sorting
        [HttpGet]
        public async Task<ActionResult> GetJobRoles([FromQuery] PaginationQuery query)
        {
            var paginatedResult = await _service.GetPaginatedAsync(query);
            return Ok(paginatedResult);
        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<JobRole>> GetJobRoleById(Guid jobId)
        {
            var job = await _service.GetByIdAsync(jobId);
            if (job == null) return NotFound();
            return Ok(job);
        }

        // ✅ Single POST endpoint with duplicate handling
        [HttpPost]
        public async Task<ActionResult<JobRole>> CreateJobRole([FromBody] JobRole jobRole)
        {
            var created = await _service.CreateAsync(jobRole);

            if (created == null)
            {
                return Conflict(new
                {
                    title = "This job already exists.",
                    status = 409
                });
            }

            return CreatedAtAction(nameof(GetJobRoleById), new { jobId = created.JobId }, created);
        }

        [HttpPut("{jobId}")]
        public async Task<IActionResult> UpdateJobRole(Guid jobId, [FromBody] JobRole jobRole)
        {
            if (jobId != jobRole.JobId)
                return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(jobId, jobRole);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{jobId}")]
        public async Task<IActionResult> DeleteJobRole(Guid jobId)
        {
            var deleted = await _service.DeleteAsync(jobId);
            return deleted ? NoContent() : NotFound();
        }
    }
}
