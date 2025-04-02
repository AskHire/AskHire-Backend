// using System;
// using System.ComponentModel.DataAnnotations;

// namespace AskHire_Backend.Models.DTOs
// {
//    public class VacancyDTO
//    {
//        [Required(ErrorMessage = "JobId is required.")]
//        public Guid JobId { get; set; }

//        [Required(ErrorMessage = "Experience is required.")]
//        public string Experience { get; set; }

//        [Required(ErrorMessage = "Education is required.")]
//        public string Education { get; set; }

//        [Required(ErrorMessage = "Non-technical skills are required.")]
//        public string NonTechnicalSkills { get; set; }

//        [Required(ErrorMessage = "Required skills are required.")]
//        public string RequiredSkills { get; set; }

//        [Required(ErrorMessage = "CV pass mark is required.")]
//        [Range(0, 100, ErrorMessage = "CV pass mark must be between 0 and 100.")]
//        public int CVPassMark { get; set; }

//        [Required(ErrorMessage = "Pre-screen pass mark is required.")]
//        [Range(0, 100, ErrorMessage = "Pre-screen pass mark must be between 0 and 100.")]
//        public int PreScreenPassMark { get; set; }

//        [Required(ErrorMessage = "Start date is required.")]
//        public DateTime StartDate { get; set; }

//        [Required(ErrorMessage = "End date is required.")]
//        public DateTime EndDate { get; set; }

//        [Required(ErrorMessage = "Duration is required.")]
//        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0.")]
//        public int Duration { get; set; }

//        [Required(ErrorMessage = "Question count is required.")]
//        [Range(1, int.MaxValue, ErrorMessage = "Question count must be greater than 0.")]
//        public int QuestionCount { get; set; }
//    }
// }
