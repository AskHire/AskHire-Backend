//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using AskHire_Backend.Models.DTOs.CandidateDTOs;

//public class GeminiHelper
//{
//    private readonly IHttpClientFactory _httpClientFactory;
//    private readonly string _geminiApiKey;
//    private readonly Dictionary<string, List<string>> _relatedDegreesCache = new();

//    public GeminiHelper(IHttpClientFactory httpClientFactory, string geminiApiKey)
//    {
//        _httpClientFactory = httpClientFactory;
//        _geminiApiKey = geminiApiKey;
//    }

//    public async Task<List<string>> GetRelatedDegreesAsync(string primaryDegree)
//    {
//        string normalizedDegree = primaryDegree.ToLower().Trim();
//        if (_relatedDegreesCache.TryGetValue(normalizedDegree, out var cachedDegrees))
//            return cachedDegrees;

//        string prompt = $@"
//As an AI expert in academic qualifications, identify degrees related to: {primaryDegree}
//Return ONLY a JSON array of strings. Example: [""Degree 1"", ""Degree 2"", ""Degree 3""]";

//        var httpClient = _httpClientFactory.CreateClient();
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

//        var content = new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json");
//        var response = await httpClient.PostAsync(
//            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}",
//            content);

//        if (response.IsSuccessStatusCode)
//        {
//            var raw = await response.Content.ReadAsStringAsync();
//            var json = JsonDocument.Parse(raw)
//                .RootElement
//                .GetProperty("candidates")[0]
//                .GetProperty("content")
//                .GetProperty("parts")[0]
//                .GetProperty("text")
//                .GetString();

//            json = json!.Replace("```json", "").Replace("```", "").Trim();
//            try
//            {
//                var related = JsonSerializer.Deserialize<List<string>>(json);
//                if (related?.Any() == true)
//                {
//                    _relatedDegreesCache[normalizedDegree] = related;
//                    return related;
//                }
//            }
//            catch { }
//        }

//        return new List<string> { "Information Technology", "Software Engineering", "Computer Science", "Information Systems" };
//    }

//    //    public async Task<CvAnalysisResult> AnalyzeCvAsync(CvAnalysisRequest request, List<string> techSkills, List<string> softSkills)
//    //    {
//    //        var httpClient = _httpClientFactory.CreateClient();
//    //        string prompt = $@"
//    //Analyze this CV:
//    //Job Title: {request.JobTitle}
//    //Required Education: {request.RequiredEducation}
//    //Related Education: {string.Join(", ", request.RelatedEducation)}
//    //Experience Requirement: {request.RequiredExperience}
//    //Skills Required: {string.Join(", ", request.RequiredSkills)}

//    //CV TEXT:
//    //{request.CvText}

//    //Return the following JSON:
//    //{{
//    //  ""educationAnalysis"": {{
//    //    ""matchedDegrees"": [],
//    //    ""degreeRelevanceScore"": 0,
//    //    ""explanation"": ""...""
//    //  }},
//    //  ""experienceAnalysis"": {{
//    //    ""relevantExperienceYears"": 0,
//    //    ""experienceScore"": 0,
//    //    ""explanation"": ""...""
//    //  }},
//    //  ""skillsAnalysis"": {{
//    //    ""matchedSkills"": [],
//    //    ""missingSkills"": [],
//    //    ""skillsScore"": 0,
//    //    ""technicalSkillsScore"": 0,
//    //    ""nonTechnicalSkillsScore"": 0
//    //  }},
//    //  ""recommendation"": ""...""
//    //}}";

//    //        var jsonPayload = new
//    //        {
//    //            contents = new[]
//    //            {
//    //                new
//    //                {
//    //                    parts = new[]
//    //                    {
//    //                        new { text = prompt }
//    //                    }
//    //                }
//    //            }
//    //        };

//    //        var response = await httpClient.PostAsync(
//    //            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}",
//    //            new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json"));

//    //        if (!response.IsSuccessStatusCode)
//    //            throw new Exception("Gemini API call failed");

//    //        string content = await response.Content.ReadAsStringAsync();
//    //        var json = JsonDocument.Parse(content).RootElement
//    //            .GetProperty("candidates")[0]
//    //            .GetProperty("content").GetProperty("parts")[0]
//    //            .GetProperty("text").GetString();

//    //        json = json!.Replace("```json", "").Replace("```", "").Trim();
//    //        var result = JsonSerializer.Deserialize<CvAnalysisResult>(json);

//    //        double score =
//    //            0.4 * result.EducationAnalysis.DegreeRelevanceScore +
//    //            0.3 * result.ExperienceAnalysis.ExperienceScore +
//    //            0.2 * result.SkillsAnalysis.TechnicalSkillsScore +
//    //            0.1 * result.SkillsAnalysis.NonTechnicalSkillsScore;

//    //        result.OverallScore = Math.Round(score*100, 2);
//    //        result.RelatedDegreesUsed = request.RelatedEducation;

//    //        return result;
//    //    }

//    //    public async Task<CvAnalysisResult> AnalyzeCvAsync(CvAnalysisRequest request, List<string> techSkills, List<string> softSkills)
//    //    {
//    //        var httpClient = _httpClientFactory.CreateClient();
//    //        string prompt = $@"
//    //Analyze this CV:
//    //Job Title: {request.JobTitle}
//    //Required Education: {request.RequiredEducation}
//    //Related Education: {string.Join(", ", request.RelatedEducation)}
//    //Experience Requirement: {request.RequiredExperience}
//    //Skills Required: {string.Join(", ", request.RequiredSkills)}

//    //CV TEXT:
//    //{request.CvText}

//    //Return the following JSON:
//    //{{
//    //  ""educationAnalysis"": {{
//    //    ""matchedDegrees"": [],
//    //    ""degreeRelevanceScore"": 0,
//    //    ""explanation"": ""...""
//    //  }},
//    //  ""experienceAnalysis"": {{
//    //    ""relevantExperienceYears"": 0,
//    //    ""experienceScore"": 0,
//    //    ""explanation"": ""...""
//    //  }},
//    //  ""skillsAnalysis"": {{
//    //    ""matchedSkills"": [],
//    //    ""missingSkills"": [],
//    //    ""skillsScore"": 0,
//    //    ""technicalSkillsScore"": 0,
//    //    ""nonTechnicalSkillsScore"": 0
//    //  }},
//    //  ""recommendation"": ""...""
//    //}}";

//    //        var jsonPayload = new
//    //        {
//    //            contents = new[]
//    //            {
//    //            new
//    //            {
//    //                parts = new[]
//    //                {
//    //                    new { text = prompt }
//    //                }
//    //            }
//    //        }
//    //        };

//    //        var response = await httpClient.PostAsync(
//    //            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}",
//    //            new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json"));

//    //        if (!response.IsSuccessStatusCode)
//    //        {
//    //            var errorContent = await response.Content.ReadAsStringAsync();
//    //            throw new Exception($"Gemini API call failed. Status: {response.StatusCode}. Content: {errorContent}");
//    //        }


//    //        string content = await response.Content.ReadAsStringAsync();
//    //        var json = JsonDocument.Parse(content).RootElement
//    //            .GetProperty("candidates")[0]
//    //            .GetProperty("content").GetProperty("parts")[0]
//    //            .GetProperty("text").GetString();

//    //        json = json!.Replace("```json", "").Replace("```", "").Trim();

//    //        if (!json.StartsWith("{"))
//    //            throw new JsonException("Gemini response did not return valid JSON content. Response: " + json);

//    //        CvAnalysisResult result;
//    //        try
//    //        {
//    //            result = JsonSerializer.Deserialize<CvAnalysisResult>(json);
//    //        }
//    //        catch (JsonException ex)
//    //        {
//    //            throw new Exception($"Failed to deserialize Gemini response: {json}", ex);
//    //        }

//    //        double score =
//    //            0.4 * result.EducationAnalysis.DegreeRelevanceScore +
//    //            0.3 * result.ExperienceAnalysis.ExperienceScore +
//    //            0.2 * result.SkillsAnalysis.TechnicalSkillsScore +
//    //            0.1 * result.SkillsAnalysis.NonTechnicalSkillsScore;

//    //        result.OverallScore = Math.Round(score, 2);
//    //        result.RelatedDegreesUsed = request.RelatedEducation;

//    //        return result;
//    //    }

//    public async Task<CvAnalysisResult> AnalyzeCvAsync(CvAnalysisRequest request, List<string> techSkills, List<string> softSkills)
//    {
//        var httpClient = _httpClientFactory.CreateClient();
//        string prompt = $@"
//Analyze this CV for the position of {request.JobTitle}:

//Required Education: {request.RequiredEducation}
//Related Education: {string.Join(", ", request.RelatedEducation)}
//Experience Requirement: {request.RequiredExperience}
//Technical Skills Required: {string.Join(", ", techSkills)}
//Non-Technical Skills Required: {string.Join(", ", softSkills)}

//CV TEXT:
//{request.CvText}

//IMPORTANT: Return scores as percentages (0-100) for degreeRelevanceScore and experienceScore.
//For skillsScore, technicalSkillsScore, and nonTechnicalSkillsScore, return as decimals (0.0-1.0).

//Return ONLY the following JSON structure:
//{{
//  ""educationAnalysis"": {{
//    ""matchedDegrees"": [""list of matched degrees""],
//    ""degreeRelevanceScore"": 100,
//    ""explanation"": ""detailed explanation""
//  }},
//  ""experienceAnalysis"": {{
//    ""relevantExperienceYears"": 0,
//    ""experienceScore"": 100,
//    ""explanation"": ""detailed explanation""
//  }},
//  ""skillsAnalysis"": {{
//    ""matchedSkills"": [""list of matched skills""],
//    ""missingSkills"": [""list of missing skills""],
//    ""skillsScore"": 0.0,
//    ""technicalSkillsScore"": 0.0,
//    ""nonTechnicalSkillsScore"": 0.0,
//    ""explanation"": ""detailed explanation""
//  }},
//  ""recommendation"": ""overall recommendation""
//}}";

//        var jsonPayload = new
//        {
//            contents = new[]
//            {
//            new
//            {
//                parts = new[]
//                {
//                    new { text = prompt }
//                }
//            }
//        }
//        };

//        var content = new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json");

//        try
//        {
//            var response = await httpClient.PostAsync(
//                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}",
//                content);

//            if (!response.IsSuccessStatusCode)
//            {
//                var errorContent = await response.Content.ReadAsStringAsync();
//                throw new Exception($"Gemini API call failed. Status: {response.StatusCode}. Content: {errorContent}");
//            }

//            string responseContent = await response.Content.ReadAsStringAsync();

//            if (string.IsNullOrWhiteSpace(responseContent))
//            {
//                throw new Exception("Gemini API returned empty response");
//            }

//            JsonDocument jsonDoc;
//            try
//            {
//                jsonDoc = JsonDocument.Parse(responseContent);
//            }
//            catch (JsonException ex)
//            {
//                throw new Exception($"Failed to parse Gemini response as JSON: {responseContent}", ex);
//            }

//            // Extract the text response with proper error handling
//            string json = null;
//            try
//            {
//                var candidates = jsonDoc.RootElement.GetProperty("candidates");
//                if (candidates.GetArrayLength() == 0)
//                {
//                    throw new Exception("No candidates returned from Gemini API");
//                }

//                var firstCandidate = candidates[0];
//                if (!firstCandidate.TryGetProperty("content", out var contentProp))
//                {
//                    throw new Exception("No content found in Gemini response");
//                }

//                var parts = contentProp.GetProperty("parts");
//                if (parts.GetArrayLength() == 0)
//                {
//                    throw new Exception("No parts found in Gemini response content");
//                }

//                json = parts[0].GetProperty("text").GetString();
//            }
//            catch (KeyNotFoundException ex)
//            {
//                throw new Exception($"Unexpected Gemini response structure: {responseContent}", ex);
//            }

//            if (string.IsNullOrWhiteSpace(json))
//            {
//                throw new Exception("Gemini returned empty text content");
//            }

//            // Clean up the JSON response
//            json = json.Replace("```json", "").Replace("```", "").Trim();

//            if (!json.StartsWith("{"))
//            {
//                throw new Exception($"Gemini response did not return valid JSON content. Response: {json}");
//            }

//            CvAnalysisResult result;
//            try
//            {
//                var options = new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                };
//                result = JsonSerializer.Deserialize<CvAnalysisResult>(json, options);
//            }
//            catch (JsonException ex)
//            {
//                throw new Exception($"Failed to deserialize Gemini response: {json}", ex);
//            }

//            // Validate the result
//            if (result == null)
//            {
//                throw new Exception("Deserialized result is null");
//            }

//            if (result.EducationAnalysis == null || result.ExperienceAnalysis == null || result.SkillsAnalysis == null)
//            {
//                throw new Exception("Incomplete analysis result from Gemini");
//            }

//            // Normalize scores to ensure they're in the correct range
//            // Education and Experience scores should be 0-100
//            var educationScore = Math.Max(0, Math.Min(100, result.EducationAnalysis.DegreeRelevanceScore));
//            var experienceScore = Math.Max(0, Math.Min(100, result.ExperienceAnalysis.ExperienceScore));

//            // Skills scores should be 0-1, convert to 0-100 for calculation
//            var technicalScore = Math.Max(0, Math.Min(1, result.SkillsAnalysis.TechnicalSkillsScore)) * 100;
//            var nonTechnicalScore = Math.Max(0, Math.Min(1, result.SkillsAnalysis.NonTechnicalSkillsScore)) * 100;

//            // Calculate overall score with proper weighting
//            double overallScore =
//                (0.4 * educationScore) +
//                (0.3 * experienceScore) +
//                (0.2 * technicalScore) +
//                (0.1 * nonTechnicalScore);

//            result.OverallScore = Math.Round(overallScore, 2);
//            result.RelatedDegreesUsed = request.RelatedEducation;

//            return result;
//        }
//        catch (HttpRequestException ex)
//        {
//            throw new Exception($"HTTP error calling Gemini API: {ex.Message}", ex);
//        }
//        catch (TaskCanceledException ex)
//        {
//            throw new Exception("Gemini API request timed out", ex);
//        }
//    }

//}

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AskHire_Backend.Models.DTOs.CandidateDTOs;

public class GeminiHelper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _geminiApiKey;
    private readonly Dictionary<string, List<string>> _relatedDegreesCache = new();

    // JSON serializer options for consistent deserialization
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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

    public async Task<CvAnalysisResult> AnalyzeCvAsync(CvAnalysisRequest request, List<string> techSkills, List<string> softSkills)
    {
        var httpClient = _httpClientFactory.CreateClient();
        string prompt = $@"
Analyze this CV for the position of {request.JobTitle}:

Required Education: {request.RequiredEducation}
Related Education: {string.Join(", ", request.RelatedEducation)}
Experience Requirement: {request.RequiredExperience}
Technical Skills Required: {string.Join(", ", techSkills)}
Non-Technical Skills Required: {string.Join(", ", softSkills)}

CV TEXT:
{request.CvText}

IMPORTANT: Return scores as percentages (0-100) for degreeRelevanceScore and experienceScore.
For skillsScore, technicalSkillsScore, and nonTechnicalSkillsScore, return as decimals (0.0-1.0).

Return ONLY the following JSON structure with no additional text or formatting:
{{
  ""educationAnalysis"": {{
    ""matchedDegrees"": [""list of matched degrees""],
    ""degreeRelevanceScore"": 100,
    ""explanation"": ""detailed explanation""
  }},
  ""experienceAnalysis"": {{
    ""relevantExperienceYears"": 0,
    ""experienceScore"": 100,
    ""explanation"": ""detailed explanation""
  }},
  ""skillsAnalysis"": {{
    ""matchedSkills"": [""list of matched skills""],
    ""missingSkills"": [""list of missing skills""],
    ""skillsScore"": 0.0,
    ""technicalSkillsScore"": 0.0,
    ""nonTechnicalSkillsScore"": 0.0,
    ""explanation"": ""detailed explanation""
  }},
  ""recommendation"": ""overall recommendation""
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

        var content = new StringContent(JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API call failed. Status: {response.StatusCode}. Content: {errorContent}");
            }

            string responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new Exception("Gemini API returned empty response");
            }

            // Parse the Gemini response structure
            string json = ExtractJsonFromGeminiResponse(responseContent);

            // Clean up the JSON response more thoroughly
            json = CleanJsonResponse(json);

            // Add detailed logging for debugging
            Console.WriteLine($"Cleaned JSON for deserialization: {json}");

            // Validate JSON structure before deserialization
            if (!IsValidJsonStructure(json))
            {
                throw new Exception($"Invalid JSON structure received: {json}");
            }

            CvAnalysisResult result;
            try
            {
                result = JsonSerializer.Deserialize<CvAnalysisResult>(json, JsonOptions);
            }
            catch (JsonException ex)
            {
                // Try with a more flexible approach
                try
                {
                    var jsonDoc = JsonDocument.Parse(json);
                    result = ParseCvAnalysisResultManually(jsonDoc);
                }
                catch (Exception manualEx)
                {
                    throw new Exception($"Failed to deserialize Gemini response both automatically and manually. JSON: {json}. Auto error: {ex.Message}. Manual error: {manualEx.Message}");
                }
            }

            // Validate the result
            ValidateAnalysisResult(result, json);

            // Calculate overall score
            CalculateOverallScore(result);
            result.RelatedDegreesUsed = request.RelatedEducation;

            return result;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"HTTP error calling Gemini API: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception("Gemini API request timed out", ex);
        }
    }

    private string ExtractJsonFromGeminiResponse(string responseContent)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(responseContent);
            var candidates = jsonDoc.RootElement.GetProperty("candidates");

            if (candidates.GetArrayLength() == 0)
            {
                throw new Exception("No candidates returned from Gemini API");
            }

            var firstCandidate = candidates[0];
            if (!firstCandidate.TryGetProperty("content", out var contentProp))
            {
                throw new Exception("No content found in Gemini response");
            }

            var parts = contentProp.GetProperty("parts");
            if (parts.GetArrayLength() == 0)
            {
                throw new Exception("No parts found in Gemini response content");
            }

            var json = parts[0].GetProperty("text").GetString();

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception("Gemini returned empty text content");
            }

            return json;
        }
        catch (KeyNotFoundException ex)
        {
            throw new Exception($"Unexpected Gemini response structure: {responseContent}", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to parse Gemini response as JSON: {responseContent}", ex);
        }
    }

    private string CleanJsonResponse(string json)
    {
        // Remove markdown code block markers
        json = json.Replace("```json", "").Replace("```", "").Trim();

        // Remove any leading/trailing whitespace and newlines
        json = json.Trim();

        // Find the first { and last } to extract just the JSON object
        int firstBrace = json.IndexOf('{');
        int lastBrace = json.LastIndexOf('}');

        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            json = json.Substring(firstBrace, lastBrace - firstBrace + 1);
        }

        return json;
    }

    private bool IsValidJsonStructure(string json)
    {
        if (!json.StartsWith("{") || !json.EndsWith("}"))
        {
            return false;
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Check for required top-level properties
            return root.TryGetProperty("educationAnalysis", out _) &&
                   root.TryGetProperty("experienceAnalysis", out _) &&
                   root.TryGetProperty("skillsAnalysis", out _) &&
                   root.TryGetProperty("recommendation", out _);
        }
        catch
        {
            return false;
        }
    }

    private CvAnalysisResult ParseCvAnalysisResultManually(JsonDocument jsonDoc)
    {
        var root = jsonDoc.RootElement;

        var result = new CvAnalysisResult
        {
            EducationAnalysis = ParseEducationAnalysis(root.GetProperty("educationAnalysis")),
            ExperienceAnalysis = ParseExperienceAnalysis(root.GetProperty("experienceAnalysis")),
            SkillsAnalysis = ParseSkillsAnalysis(root.GetProperty("skillsAnalysis")),
            Recommendation = root.GetProperty("recommendation").GetString() ?? ""
        };

        return result;
    }

    private EducationAnalysis ParseEducationAnalysis(JsonElement element)
    {
        var matchedDegrees = new List<string>();
        if (element.TryGetProperty("matchedDegrees", out var degreesArray))
        {
            foreach (var degree in degreesArray.EnumerateArray())
            {
                matchedDegrees.Add(degree.GetString() ?? "");
            }
        }

        return new EducationAnalysis
        {
            MatchedDegrees = matchedDegrees,
            DegreeRelevanceScore = element.TryGetProperty("degreeRelevanceScore", out var scoreElement)
                ? scoreElement.GetDouble() : 0,
            Explanation = element.TryGetProperty("explanation", out var explElement)
                ? explElement.GetString() ?? "" : ""
        };
    }

    private ExperienceAnalysis ParseExperienceAnalysis(JsonElement element)
    {
        return new ExperienceAnalysis
        {
            RelevantExperienceYears = element.TryGetProperty("relevantExperienceYears", out var yearsElement)
                ? yearsElement.GetInt32() : 0,
            ExperienceScore = element.TryGetProperty("experienceScore", out var scoreElement)
                ? scoreElement.GetDouble() : 0,
            Explanation = element.TryGetProperty("explanation", out var explElement)
                ? explElement.GetString() ?? "" : ""
        };
    }

    private SkillsAnalysis ParseSkillsAnalysis(JsonElement element)
    {
        var matchedSkills = new List<string>();
        if (element.TryGetProperty("matchedSkills", out var matchedArray))
        {
            foreach (var skill in matchedArray.EnumerateArray())
            {
                matchedSkills.Add(skill.GetString() ?? "");
            }
        }

        var missingSkills = new List<string>();
        if (element.TryGetProperty("missingSkills", out var missingArray))
        {
            foreach (var skill in missingArray.EnumerateArray())
            {
                missingSkills.Add(skill.GetString() ?? "");
            }
        }

        return new SkillsAnalysis
        {
            MatchedSkills = matchedSkills,
            MissingSkills = missingSkills,
            SkillsScore = element.TryGetProperty("skillsScore", out var skillsScoreElement)
                ? skillsScoreElement.GetDouble() : 0,
            TechnicalSkillsScore = element.TryGetProperty("technicalSkillsScore", out var techScoreElement)
                ? techScoreElement.GetDouble() : 0,
            NonTechnicalSkillsScore = element.TryGetProperty("nonTechnicalSkillsScore", out var nonTechScoreElement)
                ? nonTechScoreElement.GetDouble() : 0,
            Explanation = element.TryGetProperty("explanation", out var explElement)
                ? explElement.GetString() ?? "" : ""
        };
    }

    private void ValidateAnalysisResult(CvAnalysisResult result, string originalJson)
    {
        if (result == null)
        {
            throw new Exception("Deserialized result is null");
        }

        if (result.EducationAnalysis == null)
        {
            throw new Exception($"EducationAnalysis is null. Original JSON: {originalJson}");
        }

        if (result.ExperienceAnalysis == null)
        {
            throw new Exception($"ExperienceAnalysis is null. Original JSON: {originalJson}");
        }

        if (result.SkillsAnalysis == null)
        {
            throw new Exception($"SkillsAnalysis is null. Original JSON: {originalJson}");
        }
    }

    private void CalculateOverallScore(CvAnalysisResult result)
    {
        // Normalize scores to ensure they're in the correct range
        var educationScore = Math.Max(0, Math.Min(100, result.EducationAnalysis.DegreeRelevanceScore));
        var experienceScore = Math.Max(0, Math.Min(100, result.ExperienceAnalysis.ExperienceScore));

        // Skills scores should be 0-1, convert to 0-100 for calculation
        var technicalScore = Math.Max(0, Math.Min(1, result.SkillsAnalysis.TechnicalSkillsScore)) * 100;
        var nonTechnicalScore = Math.Max(0, Math.Min(1, result.SkillsAnalysis.NonTechnicalSkillsScore)) * 100;

        // Calculate overall score with proper weighting
        double overallScore =
            (0.4 * educationScore) +
            (0.3 * experienceScore) +
            (0.2 * technicalScore) +
            (0.1 * nonTechnicalScore);

        result.OverallScore = Math.Round(overallScore, 2);
    }
}



