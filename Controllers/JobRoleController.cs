using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Data.Entities;

namespace AskHire_Backend.Controllers
{
    [Route("api/jobrole")]
    [ApiController]
    public class JobRoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobRoleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/jobrole
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobRole>>> GetJobRoles()
        {
            return await _context.JobRoles.ToListAsync();
        }

        // POST: api/jobrole
        [HttpPost]
        public async Task<ActionResult<JobRole>> CreateJobRole(JobRole jobRole)
        {
            // No need to assign JobId manually anymore
            _context.JobRoles.Add(jobRole);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJobRoleById), new { jobId = jobRole.JobId }, jobRole);
        }

        // GET: api/jobrole/{jobId}
        [HttpGet("{jobId}")]
        public async Task<ActionResult<JobRole>> GetJobRoleById(Guid jobId)
        {
            var jobRole = await _context.JobRoles.FindAsync(jobId);
            if (jobRole == null)
            {
                return NotFound();
            }

            return jobRole;
        }

        // PUT: api/jobrole/{jobId}
        [HttpPut("{jobId}")]
        public async Task<IActionResult> UpdateJobRole(Guid jobId, [FromBody] JobRole updatedJobRole)
        {
            if (jobId != updatedJobRole.JobId)
            {
                return BadRequest("Job ID in the URL does not match the Job ID in the request body.");
            }

            var existingJob = await _context.JobRoles.FindAsync(jobId);
            if (existingJob == null)
            {
                return NotFound("Job role not found.");
            }

            // Update fields
            existingJob.JobTitle = updatedJobRole.JobTitle;
            existingJob.Description = updatedJobRole.Description;
            existingJob.WorkType = updatedJobRole.WorkType;
            existingJob.WorkLocation = updatedJobRole.WorkLocation;

            _context.JobRoles.Update(existingJob);
            await _context.SaveChangesAsync();

            return Ok(existingJob);
        }

        // DELETE: api/jobrole/{jobId}
        [HttpDelete("{jobId}")]
        public async Task<IActionResult> DeleteJobRole(Guid jobId)
        {
            var jobRole = await _context.JobRoles.FindAsync(jobId);
            if (jobRole == null)
            {
                return NotFound();
            }

            _context.JobRoles.Remove(jobRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
