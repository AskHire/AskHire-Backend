using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Data.Entities;

namespace AskHire_Backend.Data.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReminderRepository> _logger;

        public ReminderRepository(AppDbContext context, ILogger<ReminderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Reminder> CreateReminderAsync(Reminder reminder)
        {
            try
            {
                _context.Reminder.Add(reminder);
                await _context.SaveChangesAsync();
                return reminder;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateReminderAsync");
                throw;
            }
        }

        public async Task<Reminder?> GetReminderByIdAsync(Guid id)
        {
            try
            {
                return await _context.Reminder.FirstOrDefaultAsync(r => r.ReminderId == id);
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
                return await _context.Reminder.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllRemindersAsync");
                throw;
            }
        }

        public async Task<bool> DeleteReminderAsync(Guid id)
        {
            try
            {
                var reminder = await _context.Reminder.FindAsync(id);
                if (reminder == null)
                    return false;

                _context.Reminder.Remove(reminder);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteReminderAsync");
                throw;
            }
        }

        public async Task<Reminder?> UpdateReminderAsync(Reminder reminder)
        {
            try
            {
                var existingReminder = await _context.Reminder.FindAsync(reminder.ReminderId);
                if (existingReminder == null)
                    return null;

                _context.Entry(existingReminder).CurrentValues.SetValues(reminder);
                await _context.SaveChangesAsync();
                return existingReminder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateReminderAsync");
                throw;
            }
        }
    }
}
