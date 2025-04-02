using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.DTOs
{
    public class QuestionDTO
    {
        public Guid JobId { get; set; }

        [Required(ErrorMessage = "Question name is required.")]
        public required string QuestionName { get; set; }

        [Required(ErrorMessage = "Option1 is required.")]
        public required string Option1 { get; set; }

        [Required(ErrorMessage = "Option2 is required.")]
        public required string Option2 { get; set; }

        [Required(ErrorMessage = "Option3 is required.")]
        public required string Option3 { get; set; }

        [Required(ErrorMessage = "Option4 is required.")]
        public required string Option4 { get; set; }

        [Required(ErrorMessage = "Answer is required.")]
        public required string Answer { get; set; }
    }
}


