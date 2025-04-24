//using AskHire_Backend.Data.Entities;
//using AskHire_Backend.Models;
//using AskHire_Backend.Models.DTOs;
//using AskHire_Backend.Models.Entities;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace AskHire_Backend.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class PreScreenTestController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public PreScreenTestController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/PreScreenTest/{applicationId}
//        [HttpGet("{applicationId}")]
//        public async Task<ActionResult<PreScreenTestDto>> GetPreScreenVacancyInfo(Guid applicationId)
//        {
//            var application = await _context.Applies
//                .Include(a => a.Vacancy)
//                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

//            if (application == null || application.Vacancy == null)
//            {
//                return NotFound("Application or related Vacancy not found.");
//            }

//            var result = new PreScreenTestDto
//            {
//                VacancyName = application.Vacancy.VacancyName,
//                QuestionCount = application.Vacancy.QuestionCount,
//                Duration = application.Vacancy.Duration
//            };

//            return Ok(result);
//        }
//        // GET: api/PreScreenTest/Questions/{applicationId}
//        [HttpGet("Questions/{applicationId}")]
//        public async Task<ActionResult<PreScreenTestDto>> GetPreScreenQuestions(Guid applicationId)
//        {
//            var application = await _context.Applies
//                .Include(a => a.Vacancy)
//                .ThenInclude(v => v.JobRole)
//                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

//            if (application == null || application.Vacancy == null || application.Vacancy.JobId == null)
//            {
//                return NotFound("Application, related Vacancy, or Job Role not found.");
//            }

//            var jobId = application.Vacancy.JobId.Value;

//            var questions = await _context.Questions
//                .Where(q => q.JobId == jobId)
//                .Take(application.Vacancy.QuestionCount)
//                .ToListAsync();

//            var questionDtos = questions.Select(q => new PreScreenQuestionDto
//            {
//                QuestionId = q.QuestionId,
//                QuestionName = q.QuestionName,
//                Option1 = q.Option1,
//                Option2 = q.Option2,
//                Option3 = q.Option3,
//                Option4 = q.Option4
//            }).ToList();

//            var response = new PreScreenTestDto
//            {
//                Questions = questionDtos,
//                Duration = application.Vacancy.Duration,
//                QuestionCount = application.Vacancy.QuestionCount
//            };

//            return Ok(response);
//        }

//    }
//}

using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreScreenTestController : ControllerBase
    {
        private readonly IPreScreenTestService _service;

        public PreScreenTestController(IPreScreenTestService service)
        {
            _service = service;
        }

        [HttpGet("{applicationId}")]
        public async Task<ActionResult<PreScreenTestDto>> GetPreScreenVacancyInfo(Guid applicationId)
        {
            var result = await _service.GetVacancyInfo(applicationId);
            if (result == null)
                return NotFound("Application or related Vacancy not found.");

            return Ok(result);
        }

        [HttpGet("Questions/{applicationId}")]
        public async Task<ActionResult<PreScreenTestDto>> GetPreScreenQuestions(Guid applicationId)
        {
            var result = await _service.GetQuestions(applicationId);
            if (result == null)
                return NotFound("Application, related Vacancy, or Job Role not found.");

            return Ok(result);
        }
    }
}

