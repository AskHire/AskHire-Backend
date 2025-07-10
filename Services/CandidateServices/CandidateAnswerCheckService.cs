using System.Net;
using System.Net.Mail;
using AskHire_Backend.Interfaces.Repositories.CandidateRepositories;
using AskHire_Backend.Interfaces.Services.ICandidateServices;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Services.CandidateServices
{
    public class CandidateAnswerCheckService : ICandidateAnswerCheckService
    {
        private readonly ICandidateAnswerCheckRepository _repository;

        public CandidateAnswerCheckService(ICandidateAnswerCheckRepository repository)
        {
            _repository = repository;
        }

        public async Task<CheckAnswersResponseDto> CheckAnswersAsync(Guid applicationId, CheckAnswersRequest request)
        {
            var response = new CheckAnswersResponseDto();

            if (request.QuestionCount != request.Answers.Count)
            {
                response.Error = "The number of answers does not match the question count.";
                return response;
            }

            var application = await _repository.GetApplicationWithVacancyAsync(applicationId);
            if (application == null)
            {
                response.Error = "Application not found.";
                return response;
            }

            int correctAnswersCount = 0;
            var debugInfo = new List<object>();

            for (int i = 0; i < request.QuestionCount; i++)
            {
                var answer = request.Answers[i];
                var question = await _repository.GetQuestionByIdAsync(answer.QuestionId);
                if (question == null)
                {
                    response.Error = $"Question with ID {answer.QuestionId} not found.";
                    return response;
                }

                string normalizedProvidedAnswer = answer.Answer.Replace(" ", "");
                string normalizedCorrectAnswer = question.Answer.Replace(" ", "");

                bool isMatch = normalizedProvidedAnswer.Equals(normalizedCorrectAnswer, StringComparison.OrdinalIgnoreCase);

                if (isMatch)
                {
                    correctAnswersCount++;
                }

                debugInfo.Add(new
                {
                    questionId = answer.QuestionId,
                    providedAnswer = answer.Answer,
                    correctAnswer = question.Answer,
                    normalizedProvidedAnswer,
                    normalizedCorrectAnswer,
                    isMatch
                });
            }

            double percentage = correctAnswersCount * 100.0 / request.QuestionCount;
            application.Pre_Screen_PassMark = (int)percentage;
            application.Status = application.Pre_Screen_PassMark >= application.Vacancy.PreScreenPassMark ? "Longlist" : "Rejected";
            application.DashboardStatus = "Pre-Screening";
            await _repository.SaveChangesAsync();

            response.QuestionCount = request.QuestionCount;
            response.CorrectAnswersCount = correctAnswersCount;
            response.Pre_Screen_PassMark = (int)percentage;
            response.Status = application.Status;
            response.Debug = debugInfo;

            return response;
        }

        public Task<PreScreenPassMarkDto?> GetPreScreenPassMarkAndEmailAsync(Guid applicationId)
        {
            return _repository.GetPreScreenPassMarkAndEmailAsync(applicationId);
        }

        public async Task<bool> SendPreScreenPassMarkEmailAsync(string recipientEmail, int passMark)
        {
            try
            {
                if (string.IsNullOrEmpty(recipientEmail)) return false;

                var message = new MailMessage
                {
                    From = new MailAddress("dimashiwickramage2002@gmail.com", "AskHire Team"),
                    Subject = "Your Pre-Screen Pass Mark",
                    Body = $"Dear Candidate,\n\nYour required Pre-Screen Mark is: {passMark}/100.\n\nGood luck!\n\nBest regards,\nAskHire Team",
                    IsBodyHtml = false
                };

                message.To.Add(recipientEmail);

                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("dimashiwickramage2002@gmail.com", "fnxm msjt blvm vnmo"),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
                return false;
            }
        }



    }
}
