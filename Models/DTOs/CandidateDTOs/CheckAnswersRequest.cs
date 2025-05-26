namespace AskHire_Backend.Models.DTOs.CandidateDTOs
{
    public class CheckAnswersRequest
    {
        public int QuestionCount { get; set; }
        public List<AnswerDetail> Answers { get; set; }
    }
}
