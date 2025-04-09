using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InterviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> ScheduleInterview(InterviewScheduleRequest interviewRequest)
        {
            // Find the application instead of user
            var application = await _context.Applies.Include(a => a.User)
                .FirstOrDefaultAsync(a => a.ApplicationId == interviewRequest.ApplicationId);

            if (application == null)
            {
                return NotFound("Application not found");
            }

            // Convert string date and time to proper types
            if (!DateTime.TryParse(interviewRequest.Date, out DateTime interviewDate))
            {
                return BadRequest("Invalid date format");
            }

            if (!TimeSpan.TryParse(interviewRequest.Time, out TimeSpan interviewTime))
            {
                return BadRequest("Invalid time format");
            }

            // Save interview schedule with the correct properties according to Interview entity
            var interview = new Interview
            {
                ApplicationId = application.ApplicationId,
                Application = application,
                Date = interviewDate,
                Time = interviewTime,
                Instructions = interviewRequest.Instructions,
                CandidateEmail = application.User.Email // Assuming User has Email property
            };

            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            // Send email to candidate
            await SendInterviewEmail(interview.CandidateEmail, interview);

            return Ok("Interview scheduled and invitation sent.");
        }

        private async Task SendInterviewEmail(string candidateEmail, Interview interview)
        {
            var fromAddress = new MailAddress("youremail@example.com", "Your Company");
            var toAddress = new MailAddress(candidateEmail);
            const string subject = "Interview Invitation";

            string body = $@"Dear Candidate,

            You are invited to an interview scheduled on {interview.Date.ToShortDateString()} at {interview.Time}.

            Instructions: {interview.Instructions}

            Best regards,
            Your Company";

            using (var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dimashiwickramage2002@gmail.com", "fnxm msjt blvm vnmo"),
                EnableSsl = true
            })
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtpClient.SendMailAsync(message);
            }
        }
    }

    public class InterviewScheduleRequest
    {
        public Guid ApplicationId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Instructions { get; set; }
    }
}