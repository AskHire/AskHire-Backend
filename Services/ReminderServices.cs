using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly ILogger<ReminderService> _logger;

        public ReminderService(IReminderRepository reminderRepository, ILogger<ReminderService> logger)
        {
            _reminderRepository = reminderRepository;
            _logger = logger;
        }

        public async Task<Reminder> CreateReminderAsync(Reminder reminder)
        {
            try
            {
                return await _reminderRepository.CreateReminderAsync(reminder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateReminderAsync: {Message}", ex.InnerException?.Message ?? ex.Message);
                throw;
            }
        }

        public async Task<Reminder?> GetReminderByIdAsync(Guid id)
        {
            try
            {
                return await _reminderRepository.GetReminderByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetReminderByIdAsync");
                throw;
            }
        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync()
        {
            try
            {
                return await _reminderRepository.GetAllRemindersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllRemindersAsync");
                throw;
            }
        }

        public async Task<Reminder?> UpdateReminderAsync(Reminder reminder)
        {
            try
            {
                return await _reminderRepository.UpdateReminderAsync(reminder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateReminderAsync");
                throw;
            }
        }

        public async Task<bool> DeleteReminderAsync(Guid id)
        {
            try
            {
                return await _reminderRepository.DeleteReminderAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteReminderAsync");
                throw;
            }
        }
    }
}
