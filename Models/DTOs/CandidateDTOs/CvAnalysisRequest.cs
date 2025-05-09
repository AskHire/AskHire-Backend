using System.Collections.Generic;

public class CvAnalysisRequest
{
    public string CvText { get; set; }
    public string JobTitle { get; set; }
    public string RequiredEducation { get; set; }
    public List<string> RelatedEducation { get; set; }
    public string RequiredExperience { get; set; }
    public List<string> RequiredSkills { get; set; }
}