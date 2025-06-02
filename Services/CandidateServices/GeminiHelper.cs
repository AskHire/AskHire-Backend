using System.Net.Http;
using System.Text;
using System.Text.Json;
using AskHire_Backend.Models.DTOs.CandidateDTOs;

public class GeminiHelper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _geminiApiKey;
    private readonly Dictionary<string, List<string>> _relatedDegreesCache = new();

    public GeminiHelper(IHttpClientFactory httpClientFactory, string geminiApiKey)
    {
        _httpClientFactory = httpClientFactory;
        _geminiApiKey = geminiApiKey;
    }

    public async Task<List<string>> GetRelatedDegreesAsync(string primaryDegree)
    {
        string normalizedDegree = primaryDegree.ToLower().Trim();
        if (_relatedDegreesCache.TryGetValue(normalizedDegree, out var cachedDegrees))
            return cachedDegrees;

        string prompt = $@"
As an AI expert in academic qualifications, identify degrees related to: {primaryDegree}
Return ONLY a JSON array of strings. Example: [""Degree 1"", ""Degree 2"", ""Degree 3""]";

        var httpClient = _httpClientFactory.CreateClient();
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
        var response = await httpClient.PostAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}",
            content);

        if (response.IsSuccessStatusCode)
        {
            var raw = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(raw)
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            json = json!.Replace("```json", "").Replace("```", "").Trim();
            try
            {
                var related = JsonSerializer.Deserialize<List<string>>(json);
                if (related?.Any() == true)
                {
                    _relatedDegreesCache[normalizedDegree] = related;
                    return related;
                }
            }
            catch { }
        }

        return new List<string> { "Information Technology", "Software Engineering", "Computer Science", "Information Systems" };
    }

    //    public async Task<CvAnalysisResult> AnalyzeCvAsync(CvAnalysisRequest request, List<string> techSkills, List<string> softSkills)
    //    {
    //        var httpClient = _httpClientFactory.CreateClient();
    //        string prompt = $@"
    //Analyze this CV:
    //Job Title: {request.JobTitle}
    //Required Education: {request.RequiredEducation}
    //Related Education: {string.Join(", ", request.RelatedEducation)}
    //Experience Requirement: {request.RequiredExperience}
    //Skills Required: {string.Join(", ", request.RequiredSkills)}

    //CV TEXT:
    //{request.CvText}

    //Return the following JSON:
    //{{
    //  ""educationAnalysis"": {{
    //    ""matchedDegrees"": [],
    //    ""degreeRelevanceScore"": 0,
    //    ""explanation"": ""...""
    //  }},
    //  ""experienceAnalysis"": {{
    //    ""relevantExperienceYears"": 0,
    //    ""experienceScore"": 0,
    //    ""explanation"": ""...""
    //  }},
    //  ""skillsAnalysis"": {{
    //    ""matchedSkills"": [],
    //    ""missingSkills"": [],
    //    ""skillsScore"": 0,
    //    ""technicalSkillsScore"": 0,
    //    ""nonTechnicalSkillsScore"": 0
    //  }},
    //  ""recommendation"": ""...""
    //}}";

    //        var jsonPayload = new
    //        {
    //            contents = new[]
    //            {
    //                new
    //                {
    //                    parts = new[]
    //                    {
    //                        new { text = prompt }
    //                    }
    //                }
    //            }
    //        };

    //        var response = await httpClient.PostAsync(
    //            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}",
    //            new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json"));

    //        if (!response.IsSuccessStatusCode)
    //            throw new Exception("Gemini API call failed");

    //        string content = await response.Content.ReadAsStringAsync();
    //        var json = JsonDocument.Parse(content).RootElement
    //            .GetProperty("candidates")[0]
    //            .GetProperty("content").GetProperty("parts")[0]
    //            .GetProperty("text").GetString();

    //        json = json!.Replace("```json", "").Replace("```", "").Trim();
    //        var result = JsonSerializer.Deserialize<CvAnalysisResult>(json);

    //        double score =
    //            0.4 * result.EducationAnalysis.DegreeRelevanceScore +
    //            0.3 * result.ExperienceAnalysis.ExperienceScore +
    //            0.2 * result.SkillsAnalysis.TechnicalSkillsScore +
    //            0.1 * result.SkillsAnalysis.NonTechnicalSkillsScore;

    //        result.OverallScore = Math.Round(score*100, 2);
    //        result.RelatedDegreesUsed = request.RelatedEducation;

    //        return result;
    //    }

    public async Task<CvAnalysisResult> AnalyzeCvAsync(CvAnalysisRequest request, List<string> techSkills, List<string> softSkills)
    {
        var httpClient = _httpClientFactory.CreateClient();
        string prompt = $@"
Analyze this CV:
Job Title: {request.JobTitle}
Required Education: {request.RequiredEducation}
Related Education: {string.Join(", ", request.RelatedEducation)}
Experience Requirement: {request.RequiredExperience}
Skills Required: {string.Join(", ", request.RequiredSkills)}

CV TEXT:
{request.CvText}

Return the following JSON:
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
}}";

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

        var response = await httpClient.PostAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}",
            new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API call failed. Status: {response.StatusCode}. Content: {errorContent}");
        }


        string content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content").GetProperty("parts")[0]
            .GetProperty("text").GetString();

        json = json!.Replace("```json", "").Replace("```", "").Trim();

        if (!json.StartsWith("{"))
            throw new JsonException("Gemini response did not return valid JSON content. Response: " + json);

        CvAnalysisResult result;
        try
        {
            result = JsonSerializer.Deserialize<CvAnalysisResult>(json);
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to deserialize Gemini response: {json}", ex);
        }

        double score =
            0.4 * result.EducationAnalysis.DegreeRelevanceScore +
            0.3 * result.ExperienceAnalysis.ExperienceScore +
            0.2 * result.SkillsAnalysis.TechnicalSkillsScore +
            0.1 * result.SkillsAnalysis.NonTechnicalSkillsScore;

        result.OverallScore = Math.Round(score, 2);
        result.RelatedDegreesUsed = request.RelatedEducation;

        return result;
    }

}



