using AskHire_Backend.Models.DTOs.CandidateDTOs;

public class PreScreenTestDto
{
    public List<PreScreenQuestionDto> Questions { get; set; } = new();
    public string VacancyName { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int QuestionCount { get; set; }
    public string? Status { get; set; }
}