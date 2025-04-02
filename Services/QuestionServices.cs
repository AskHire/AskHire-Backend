using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            return await _questionRepository.GetAllQuestionsAsync();
        }

        public async Task<Question?> GetQuestionByIdAsync(Guid id)
        {
            return await _questionRepository.GetQuestionByIdAsync(id);
        }

        public async Task<Question> CreateQuestionAsync(QuestionDTO questionDTO)
        {
            if (string.IsNullOrWhiteSpace(questionDTO.QuestionName))
            {
                throw new ArgumentException("Question name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(questionDTO.Option1) || 
                string.IsNullOrWhiteSpace(questionDTO.Option2) ||
                string.IsNullOrWhiteSpace(questionDTO.Option3) ||
                string.IsNullOrWhiteSpace(questionDTO.Option4))
            {
                throw new ArgumentException("All options must have values.");
            }

            if (string.IsNullOrWhiteSpace(questionDTO.Answer))
            {
                throw new ArgumentException("Answer cannot be empty.");
            }

            return await _questionRepository.CreateQuestionAsync(questionDTO);
        }

        public async Task<Question> UpdateQuestionAsync(Guid id, QuestionDTO questionDTO)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid question ID.");
            }

            if (string.IsNullOrWhiteSpace(questionDTO.QuestionName))
            {
                throw new ArgumentException("Question name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(questionDTO.Option1) || 
                string.IsNullOrWhiteSpace(questionDTO.Option2) ||
                string.IsNullOrWhiteSpace(questionDTO.Option3) ||
                string.IsNullOrWhiteSpace(questionDTO.Option4))
            {
                throw new ArgumentException("All options must have values.");
            }

            if (string.IsNullOrWhiteSpace(questionDTO.Answer))
            {
                throw new ArgumentException("Answer cannot be empty.");
            }

            return await _questionRepository.UpdateQuestionAsync(id, questionDTO);
        }

        public async Task DeleteQuestionAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid question ID.");
            }

            await _questionRepository.DeleteQuestionAsync(id);
        }
    }
}