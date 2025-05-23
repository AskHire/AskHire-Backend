using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.DTOs;
using System;
using System.Threading.Tasks;
using AskHire_Backend.Models.DTOs.ManagerDTOs;
using AskHire_Backend.Interfaces.Services.IManagerServices;
using AskHire_Backend.Services.Interfaces;

namespace AskHire_Backend.Controllers.Manager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerInterviewController : ControllerBase
    {
        private readonly IManagerInterviewService _interviewService;
        public ManagerInterviewController(IManagerInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleInterview([FromBody] ManagerInterviewScheduleRequestDTO interviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid interview data.");

            var result = await _interviewService.ScheduleInterviewAsync(interviewDto);

            if (!result)
                return BadRequest("Failed to schedule interview.");

            return Ok("Interview scheduled successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInterview(Guid id, ManagerInterviewScheduleRequestDTO interviewRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _interviewService.UpdateInterviewAsync(id, interviewRequest);
            if (!result)
            {
                return BadRequest("Failed to update interview.");
            }
            return Ok("Interview updated and notification sent.");
        }

        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> GetInterviewByApplicationId(Guid applicationId)
        {
            var interview = await _interviewService.GetInterviewByApplicationIdAsync(applicationId);
            if (interview == null)
            {
                return NotFound("Interview not found.");
            }
            return Ok(interview);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInterviews()
        {
            var interviews = await _interviewService.GetAllInterviewsAsync();
            return Ok(interviews);
        }
    }
}