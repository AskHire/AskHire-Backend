namespace AskHire_Backend.Models.DTOs.CandidateDTOs
{
    public class CheckAnswersResponseDto
    {
        public int QuestionCount { get; set; }
        public int CorrectAnswersCount { get; set; }
        public int Pre_Screen_PassMark { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<object> Debug { get; set; } = new();
        public string? Error { get; set; } 
    }
}
