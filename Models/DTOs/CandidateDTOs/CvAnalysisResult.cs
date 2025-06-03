//using System.Text.Json.Serialization;
//using System.Collections.Generic;

//namespace AskHire_Backend.Models.DTOs.CandidateDTOs
//{
//    public class CvAnalysisResult
//    {
//        [JsonPropertyName("educationAnalysis")]
//        public EducationAnalysis EducationAnalysis { get; set; }

//        [JsonPropertyName("experienceAnalysis")]
//        public ExperienceAnalysis ExperienceAnalysis { get; set; }

//        [JsonPropertyName("skillsAnalysis")]
//        public SkillsAnalysis SkillsAnalysis { get; set; }

//        [JsonPropertyName("overallScore")]
//        public double OverallScore { get; set; } 

//        [JsonPropertyName("recommendation")]
//        public string Recommendation { get; set; }

//        // Additional property 
//        public List<string> RelatedDegreesUsed { get; set; }
//    }

//    public class EducationAnalysis
//    {
//        [JsonPropertyName("matchedDegrees")]
//        public List<string> MatchedDegrees { get; set; }

//        [JsonPropertyName("degreeRelevanceScore")]
//        public double DegreeRelevanceScore { get; set; }

//        [JsonPropertyName("explanation")]
//        public string Explanation { get; set; }
//    }

//    public class ExperienceAnalysis
//    {
//        [JsonPropertyName("relevantExperienceYears")]
//        public int RelevantExperienceYears { get; set; }

//        [JsonPropertyName("experienceScore")]
//        public double ExperienceScore { get; set; }

//        [JsonPropertyName("explanation")]
//        public string Explanation { get; set; }
//    }

//    public class SkillsAnalysis
//    {

//        [JsonPropertyName("technicalSkillsScore")]
//        public int TechnicalSkillsScore { get; set; }

//        [JsonPropertyName("nonTechnicalSkillsScore")]
//        public int NonTechnicalSkillsScore { get; set; }
//    }
//}

using System.Text.Json.Serialization;

namespace AskHire_Backend.Models.DTOs.CandidateDTOs
{
    public class CvAnalysisRequest
    {
        public string JobTitle { get; set; } = string.Empty;
        public string RequiredEducation { get; set; } = string.Empty;
        public List<string> RelatedEducation { get; set; } = new();
        public string RequiredExperience { get; set; } = string.Empty;
        public List<string> RequiredSkills { get; set; } = new();
        public string CvText { get; set; } = string.Empty;
    }

    public class CvAnalysisResult
    {
        [JsonPropertyName("educationAnalysis")]
        public EducationAnalysis EducationAnalysis { get; set; } = new();

        [JsonPropertyName("experienceAnalysis")]
        public ExperienceAnalysis ExperienceAnalysis { get; set; } = new();

        [JsonPropertyName("skillsAnalysis")]
        public SkillsAnalysis SkillsAnalysis { get; set; } = new();

        [JsonPropertyName("recommendation")]
        public string Recommendation { get; set; } = string.Empty;

        // These are calculated properties, not from JSON
        public double OverallScore { get; set; }
        public List<string> RelatedDegreesUsed { get; set; } = new();
    }

    public class EducationAnalysis
    {
        [JsonPropertyName("matchedDegrees")]
        public List<string> MatchedDegrees { get; set; } = new();

        [JsonPropertyName("degreeRelevanceScore")]
        public double DegreeRelevanceScore { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;
    }

    public class ExperienceAnalysis
    {
        [JsonPropertyName("relevantExperienceYears")]
        public int RelevantExperienceYears { get; set; }

        [JsonPropertyName("experienceScore")]
        public double ExperienceScore { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;
    }

    public class SkillsAnalysis
    {
        [JsonPropertyName("matchedSkills")]
        public List<string> MatchedSkills { get; set; } = new();

        [JsonPropertyName("missingSkills")]
        public List<string> MissingSkills { get; set; } = new();

        [JsonPropertyName("skillsScore")]
        public double SkillsScore { get; set; }

        [JsonPropertyName("technicalSkillsScore")]
        public double TechnicalSkillsScore { get; set; }

        [JsonPropertyName("nonTechnicalSkillsScore")]
        public double NonTechnicalSkillsScore { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;
    }
}