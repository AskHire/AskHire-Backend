using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace AskHire_Backend.Models.DTOs.CandidateDTOs
{
    public class CvAnalysisResult
    {
        [JsonPropertyName("educationAnalysis")]
        public EducationAnalysis EducationAnalysis { get; set; }

        [JsonPropertyName("experienceAnalysis")]
        public ExperienceAnalysis ExperienceAnalysis { get; set; }

        [JsonPropertyName("skillsAnalysis")]
        public SkillsAnalysis SkillsAnalysis { get; set; }

        [JsonPropertyName("overallScore")]
        public double OverallScore { get; set; } 

        [JsonPropertyName("recommendation")]
        public string Recommendation { get; set; }

        // Additional property 
        public List<string> RelatedDegreesUsed { get; set; }
    }

    public class EducationAnalysis
    {
        [JsonPropertyName("matchedDegrees")]
        public List<string> MatchedDegrees { get; set; }

        [JsonPropertyName("degreeRelevanceScore")]
        public double DegreeRelevanceScore { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; }
    }

    public class ExperienceAnalysis
    {
        [JsonPropertyName("relevantExperienceYears")]
        public int RelevantExperienceYears { get; set; }

        [JsonPropertyName("experienceScore")]
        public double ExperienceScore { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; }
    }

    public class SkillsAnalysis
    {

        [JsonPropertyName("technicalSkillsScore")]
        public int TechnicalSkillsScore { get; set; }

        [JsonPropertyName("nonTechnicalSkillsScore")]
        public int NonTechnicalSkillsScore { get; set; }
    }
}