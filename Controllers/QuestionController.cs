using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(IQuestionService questionService, ILogger<QuestionController> logger)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/question
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsAsync();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving questions.");
                return StatusCode(500, new { message = "An internal server error occurred.", details = ex.Message });
            }
        }

        // GET: api/question/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(Guid id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    _logger.LogWarning($"Question with ID {id} not found.");
                    return NotFound(new { message = "Question not found." });
                }

                return Ok(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving question with ID {id}.");
                return StatusCode(500, new { message = "An internal server error occurred.", details = ex.Message });
            }
        }

        // POST: api/question
        [HttpPost]
        public async Task<ActionResult<Question>> PostQuestion([FromBody] QuestionDTO questionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var question = await _questionService.CreateQuestionAsync(questionDTO);
                return CreatedAtAction(nameof(GetQuestion), new { id = question.QuestionId }, question);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid data provided.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating question.");
                return StatusCode(500, new { message = "An internal server error occurred.", details = ex.Message });
            }
        }

        // PUT: api/question/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(Guid id, [FromBody] QuestionDTO questionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedQuestion = await _questionService.UpdateQuestionAsync(id, questionDTO);
                return Ok(updatedQuestion);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"PUT request failed. Question ID {id} not found. {ex.Message}");
                return NotFound(new { message = "Question not found." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid data provided for update.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the question.");
                return StatusCode(500, new { message = "An internal error occurred.", details = ex.Message });
            }
        }

        // DELETE: api/question/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"DELETE request failed. Question ID {id} not found. {ex.Message}");
                return NotFound(new { message = "Question not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the question.");
                return StatusCode(500, new { message = "An internal error occurred.", details = ex.Message });
            }
        }
    }
}