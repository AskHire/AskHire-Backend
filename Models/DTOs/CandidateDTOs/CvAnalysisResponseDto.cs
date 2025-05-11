using System;
using AskHire_Backend.Models.DTOs.CandidateDTOs;

public class CvAnalysisResponseDto
{
    public Guid ApplicationId { get; set; }
    public int CvMark { get; set; }
    public string Status { get; set; }
    public string DashboardStatus { get; set; }
    public CvAnalysisResult AnalysisDetails { get; set; }
}
