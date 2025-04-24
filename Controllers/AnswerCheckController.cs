//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using AskHire_Backend.Models.Entities;
//using AskHire_Backend.Data.Entities;
//using AskHire_Backend.Models.DTOs;

//namespace AskHire_Backend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TestController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public TestController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("check-answers/{applicationId}")]
//        public async Task<IActionResult> CheckAnswers(Guid applicationId, [FromBody] CheckAnswersRequest request)
//        {
//            if (request.QuestionCount != request.Answers.Count)
//            {
//                return BadRequest("The number of answers does not match the question count.");
//            }

//            // Retrieve the Application with related Vacancy
//            var application = await _context.Applies
//                .Include(a => a.Vacancy)
//                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

//            if (application == null)
//            {
//                return NotFound("Application not found.");
//            }

//            int correctAnswersCount = 0;

//            // Loop through each answer in the request
//            for (int i = 0; i < request.QuestionCount; i++)
//            {
//                var questionId = request.Answers[i].QuestionId;
//                var providedAnswer = request.Answers[i].Answer;

//                // Retrieve the correct answer for the question
//                var question = await _context.Questions
//                    .FirstOrDefaultAsync(q => q.QuestionId == questionId);

//                if (question == null)
//                {
//                    return NotFound($"Question with ID {questionId} not found.");
//                }

//                // Check if the provided answer matches the correct answer
//                if (providedAnswer.Equals(question.Answer, StringComparison.OrdinalIgnoreCase))
//                {
//                    correctAnswersCount++;
//                }
//            }

//            // Calculate percentage and save to Pre_Screen_PassMark
//            double percentage = correctAnswersCount * 100.0 / request.QuestionCount;
//            application.Pre_Screen_PassMark = (int)percentage;

//            await _context.SaveChangesAsync();

//            // Compare with Vacancy's PreScreenPassMark
//            var passStatus = application.Pre_Screen_PassMark >= application.Vacancy.PreScreenPassMark ? "pass" : "fail";

//            return Ok(new
//            {
//                QuestionCount = request.QuestionCount,
//                CorrectAnswersCount = correctAnswersCount,
//                application.Pre_Screen_PassMark,
//                Status = passStatus
//            });
//        }


//    }
//}


//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using AskHire_Backend.Models.Entities;
//using AskHire_Backend.Data.Entities;
//using AskHire_Backend.Models.DTOs;
//namespace AskHire_Backend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TestController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        public TestController(AppDbContext context)
//        {
//            _context = context;
//        }
//        [HttpPost("check-answers/{applicationId}")]
//        public async Task<IActionResult> CheckAnswers(Guid applicationId, [FromBody] CheckAnswersRequest request)
//        {
//            if (request.QuestionCount != request.Answers.Count)
//            {
//                return BadRequest("The number of answers does not match the question count.");
//            }
//            // Retrieve the Application with related Vacancy
//            var application = await _context.Applies
//                .Include(a => a.Vacancy)
//                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
//            if (application == null)
//            {
//                return NotFound("Application not found.");
//            }
//            int correctAnswersCount = 0;
//            var debugInfo = new List<object>();

//            // Loop through each answer in the request
//            for (int i = 0; i < request.QuestionCount; i++)
//            {
//                var questionId = request.Answers[i].QuestionId;
//                var providedAnswer = request.Answers[i].Answer;

//                // Retrieve the correct answer for the question
//                var question = await _context.Questions
//                    .FirstOrDefaultAsync(q => q.QuestionId == questionId);
//                if (question == null)
//                {
//                    return NotFound($"Question with ID {questionId} not found.");
//                }

//                // Normalize answers by removing spaces for comparison
//                string normalizedProvidedAnswer = providedAnswer.Replace(" ", "");
//                string normalizedCorrectAnswer = question.Answer.Replace(" ", "");

//                // Check if the normalized answers match
//                bool isMatch = normalizedProvidedAnswer.Equals(normalizedCorrectAnswer, StringComparison.OrdinalIgnoreCase);

//                if (isMatch)
//                {
//                    correctAnswersCount++;
//                }

//                // Add debug information
//                debugInfo.Add(new
//                {
//                    questionId = questionId.ToString(),
//                    providedAnswer,
//                    correctAnswer = question.Answer,
//                    normalizedProvidedAnswer,
//                    normalizedCorrectAnswer,
//                    isMatch
//                });
//            }

//            // Calculate percentage and save to Pre_Screen_PassMark
//            double percentage = correctAnswersCount * 100.0 / request.QuestionCount;
//            application.Pre_Screen_PassMark = (int)percentage;
//            await _context.SaveChangesAsync();

//            // Compare with Vacancy's PreScreenPassMark
//            var passStatus = application.Pre_Screen_PassMark >= application.Vacancy.PreScreenPassMark ? "pass" : "fail";

//            return Ok(new
//            {
//                QuestionCount = request.QuestionCount,
//                CorrectAnswersCount = correctAnswersCount,
//                application.Pre_Screen_PassMark,
//                Status = passStatus,
//                Debug = debugInfo
//            });
//        }
//    }
//}

using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerCheckController : ControllerBase
    {
        private readonly IAnswerCheckService _service;

        public AnswerCheckController(IAnswerCheckService service)
        {
            _service = service;
        }

        [HttpPost("mcq/{applicationId}")]
        public async Task<IActionResult> CheckAnswers(Guid applicationId, [FromBody] CheckAnswersRequest request)
        {
            var result = await _service.CheckAnswersAsync(applicationId, request);

            if (!string.IsNullOrEmpty(result.Error))
            {
                return BadRequest(result.Error);
            }

            return Ok(result);
        }
    }
}

