using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs.CandidateDTOs;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CvTallyController : ControllerBase
    {
        private const string GEMINI_ENDPOINT = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _geminiApiKey;
        private static Dictionary<string, List<string>> _relatedDegreesCache = new();

        public CvTallyController(AppDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _geminiApiKey = configuration["GeminiApiKey"];
        }

        [HttpPost("analyze-application/{applicationId}")]
        public async Task<IActionResult> AnalyzeApplication(Guid applicationId, [FromBody] CvUploadDto cvUpload)
        {
            try
            {
                var application = await _context.Applies
                    .Include(a => a.Vacancy)
                    .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

                if (application == null)
                    return NotFound($"Application with ID {applicationId} not found");

                var vacancy = application.Vacancy;
                if (vacancy == null)
                    return BadRequest("Vacancy information not available for this application");

                var requiredSkills = vacancy.RequiredSkills?.Split(',').Select(s => s.Trim()).ToList() ?? new();
                var nonTechnicalSkills = vacancy.NonTechnicalSkills?.Split(',').Select(s => s.Trim()).ToList() ?? new();
                var allRequiredSkills = requiredSkills.Concat(nonTechnicalSkills).ToList();

                var analyzeRequest = new CvAnalysisRequest
                {
                    CvText = cvUpload.CvText,
                    JobTitle = vacancy.VacancyName,
                    RequiredEducation = vacancy.Education,
                    RelatedEducation = await GetRelatedDegrees(vacancy.Education),
                    RequiredExperience = vacancy.Experience,
                    RequiredSkills = allRequiredSkills
                };

                var analysisResult = await AnalyzeCvWithGemini(analyzeRequest, requiredSkills, nonTechnicalSkills);

                application.CV_Mark = (int)Math.Round(analysisResult.OverallScore * 100);

                if (analysisResult.OverallScore >= vacancy.CVPassMark)
                {
                    application.Status = "CV Approved";
                    application.DashboardStatus = "Awaiting Pre-Screen";
                }
                else
                {
                    application.Status = "CV Rejected";
                    application.DashboardStatus = "Application Closed";
                }

                await _context.SaveChangesAsync();

                return Ok(new CvAnalysisResponseDto
                {
                    ApplicationId = application.ApplicationId,
                    CvMark = application.CV_Mark,
                    Status = application.Status,
                    DashboardStatus = application.DashboardStatus,
                    AnalysisDetails = analysisResult
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<List<string>> GetRelatedDegrees(string primaryDegree)
        {
            string normalizedDegree = primaryDegree.ToLower().Trim();

            if (_relatedDegreesCache.TryGetValue(normalizedDegree, out var cachedDegrees))
                return cachedDegrees;

            var httpClient = _httpClientFactory.CreateClient();

            string prompt = $@"
As an AI expert in academic qualifications and job requirements, identify all degrees that would be considered equivalent or related to the following primary degree requirement:
Primary Degree: {primaryDegree}
Please provide a list of at least 6-10 related degrees that would be considered acceptable alternatives in the job market.
Return ONLY a JSON array of strings with no explanation or additional text. Format example: [""Degree 1"", ""Degree 2"", ""Degree 3""]";

            var jsonPayload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{GEMINI_ENDPOINT}?key={_geminiApiKey}", content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);

                string relatedDegreesJson = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                if (!string.IsNullOrEmpty(relatedDegreesJson))
                {
                    try
                    {
                        relatedDegreesJson = relatedDegreesJson.Replace("```json", "").Replace("```", "").Trim();
                        var relatedDegrees = JsonSerializer.Deserialize<List<string>>(relatedDegreesJson);
                        if (relatedDegrees != null && relatedDegrees.Any())
                        {
                            _relatedDegreesCache[normalizedDegree] = relatedDegrees;
                            return relatedDegrees;
                        }
                    }
                    catch (JsonException) { }
                }
            }

            var defaultDegrees = new List<string> { "Information Technology", "Software Engineering", "Computer Science", "Information Systems" };
            _relatedDegreesCache[normalizedDegree] = defaultDegrees;
            return defaultDegrees;
        }

        private async Task<CvAnalysisResult> AnalyzeCvWithGemini(CvAnalysisRequest request, List<string> technicalSkills, List<string> nonTechnicalSkills)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string prompt = $@"
You are an AI talent evaluator specialized in analyzing resumes/CVs against job requirements.

JOB DETAILS:
- Job Title: {request.JobTitle}
- Primary Education Requirement: {request.RequiredEducation}
- Acceptable Related Education: {string.Join(", ", request.RelatedEducation)}
- Experience Requirement: {request.RequiredExperience}
- Additional Skills Required: {string.Join(", ", request.RequiredSkills)}

CANDIDATE CV:
{request.CvText}

Please analyze this CV against the job requirements and provide the following in JSON format:
{{
  ""educationAnalysis"": {{
    ""matchedDegrees"": [],
    ""degreeRelevanceScore"": 0,
    ""explanation"": ""...""
  }},
  ""experienceAnalysis"": {{
    ""relevantExperienceYears"": 0,
    ""experienceScore"": 0,
    ""explanation"": ""...""
  }},
  ""skillsAnalysis"": {{
    ""matchedSkills"": [],
    ""missingSkills"": [],
    ""skillsScore"": 0,
    ""technicalSkillsScore"": 0,
    ""nonTechnicalSkillsScore"": 0
  }},
  ""recommendation"": ""...""
}}

Return ONLY the JSON without any additional text or markdown.";

            var jsonPayload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{GEMINI_ENDPOINT}?key={_geminiApiKey}", content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);

                string analysisJson = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                if (!string.IsNullOrEmpty(analysisJson))
                {
                    try
                    {
                        analysisJson = analysisJson.Replace("```json", "").Replace("```", "").Trim();
                        var analysisResult = JsonSerializer.Deserialize<CvAnalysisResult>(analysisJson);

                        // Manual score calculation based on weights
                        double overallScore =
                              0.4 * (analysisResult.EducationAnalysis.DegreeRelevanceScore / 100.0)
                            + 0.3 * (analysisResult.ExperienceAnalysis.ExperienceScore / 100.0)
                            + 0.2 * (analysisResult.SkillsAnalysis.TechnicalSkillsScore / 100.0)
                            + 0.1 * (analysisResult.SkillsAnalysis.NonTechnicalSkillsScore / 100.0);

                        //analysisResult.OverallScore = overallScore;
                        analysisResult.OverallScore = Math.Round(overallScore * 100, 2); 

                        analysisResult.RelatedDegreesUsed = request.RelatedEducation;

                        return analysisResult;
                    }
                    catch (JsonException ex)
                    {
                        throw new Exception("Failed to parse Gemini response: " + ex.Message);
                    }
                }
            }

            throw new Exception("Gemini API call failed or returned invalid response.");
        }
    }
}
