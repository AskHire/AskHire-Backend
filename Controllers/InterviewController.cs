using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleInterview(InterviewScheduleRequestDTO interviewRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _interviewService.ScheduleInterviewAsync(interviewRequest);
            if (!result)
            {
                return BadRequest("Failed to schedule interview.");
            }

            return Ok("Interview scheduled and invitation sent.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInterview(Guid id, InterviewScheduleRequestDTO interviewRequest)
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

        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<List<UserInterviewDetailsDto>>> GetInterviewsByUserId(Guid userId)
        {
            var interviews = await _interviewService.GetInterviewsByUserIdAsync(userId);
            if (interviews == null || interviews.Count == 0)
            {
                return NotFound("No interviews found for the given user ID.");
            }

            return Ok(interviews);
        }
    }
}