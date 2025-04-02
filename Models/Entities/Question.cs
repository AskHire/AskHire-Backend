using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.Entities
{ 
    public class Question 
    { 
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public Guid QuestionId { get; set; } 
 
        [ForeignKey("JobRole")] 
        public Guid JobId { get; set; } 
         
        // Mark as nullable to fix the compiler error 
        public JobRole? JobRole { get; set; } 
 
        public required string QuestionName { get; set; } 
 
        public required string Option1 { get; set; } 
        public required string Option2 { get; set; } 
        public required string Option3 { get; set; } 
        public required string Option4 { get; set; } 
        public required string Answer { get; set; } 
    } 
}
