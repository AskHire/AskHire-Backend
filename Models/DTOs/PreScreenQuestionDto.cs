namespace AskHire_Backend.Models.DTOs
{
    public class PreScreenQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionName { get; set; } = string.Empty;
        public string Option1 { get; set; } = string.Empty;
        public string Option2 { get; set; } = string.Empty;
        public string Option3 { get; set; } = string.Empty;
        public string Option4 { get; set; } = string.Empty;

    }
}
