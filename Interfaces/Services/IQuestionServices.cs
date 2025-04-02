using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllQuestionsAsync();
        Task<Question?> GetQuestionByIdAsync(Guid id);
        Task<Question> CreateQuestionAsync(QuestionDTO questionDTO);
        Task<Question> UpdateQuestionAsync(Guid id, QuestionDTO questionDTO);
        Task DeleteQuestionAsync(Guid id);
    }
}