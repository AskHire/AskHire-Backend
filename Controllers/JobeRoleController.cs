using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobRoleController : ControllerBase
    {
        private readonly IJobRoleService _jobRoleService;
        private readonly ILogger<JobRoleController> _logger;

        public JobRoleController(IJobRoleService jobRoleService, ILogger<JobRoleController> logger)
        {
            _jobRoleService = jobRoleService ?? throw new ArgumentNullException(nameof(jobRoleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/JobRole
        [HttpGet]
        public async Task<IActionResult> GetJobRoles()
        {
            try
            {
                var jobRoles = await _jobRoleService.GetAllJobRolesAsync();
                return Ok(jobRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all job roles");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/JobRole/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobRoleById(Guid id)
        {
            try
            {
                var jobRole = await _jobRoleService.GetJobRoleByIdAsync(id);
                if (jobRole == null)
                {
                    _logger.LogWarning($"Job role with ID {id} not found");
                    return NotFound(new { message = "Job role not found" });
                }
                return Ok(jobRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving job role with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/JobRole
        [HttpPost]
        public async Task<ActionResult<JobRole>> CreateJobRole([FromBody] JobRole jobRole)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdJobRole = await _jobRoleService.CreateJobRoleAsync(jobRole);
                return CreatedAtAction(nameof(GetJobRoleById), new { id = createdJobRole.JobId }, createdJobRole);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null job role data provided");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating job role");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/JobRole/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobRole(Guid id, [FromBody] JobRole jobRole)
        {
            if (id != jobRole.JobId)
            {
                _logger.LogWarning($"Job role ID mismatch: URL ID={id}, Body ID={jobRole.JobId}");
                return BadRequest(new { message = "Job role ID mismatch." });
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedJobRole = await _jobRoleService.UpdateJobRoleAsync(jobRole);
                if (updatedJobRole == null)
                {
                    _logger.LogWarning($"Job role with ID {id} not found for update");
                    return NotFound(new { message = "Job role not found." });
                }
                return Ok(updatedJobRole);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null job role data provided for update");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating job role with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/JobRole/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobRole(Guid id)
        {
            try
            {
                var success = await _jobRoleService.DeleteJobRoleAsync(id);
                if (!success)
                {
                    _logger.LogWarning($"Job role with ID {id} not found for deletion");
                    return NotFound(new { message = "Job role not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting job role with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}