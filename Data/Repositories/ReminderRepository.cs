using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Data.Entities;

namespace AskHire_Backend.Data.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly AppDbContext _context;

        public ReminderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Reminder> CreateReminderAsync(Reminder reminder)
        {
            _context.Reminder.Add(reminder);  // Adding new reminder
            await _context.SaveChangesAsync();  // Save changes to the database
            return reminder;  // Return the created reminder
        }

        public async Task<Reminder?> GetReminderByIdAsync(Guid id)
        {
            return await _context.Reminder
                .FirstOrDefaultAsync(r => r.ReminderId == id);  // Retrieve reminder by ID
        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync()
        {
            return await _context.Reminder.ToListAsync();  // Retrieve all reminders
        }

        public async Task<bool> DeleteReminderAsync(Guid id)
        {
            var reminder = await _context.Reminder.FindAsync(id);  // Find reminder by ID
            if (reminder == null)
            {
                return false;  // If reminder doesn't exist, return false
            }

            _context.Reminder.Remove(reminder);  // Remove the reminder
            await _context.SaveChangesAsync();  // Save changes to the database
            return true;  // Return true if deletion was successful
        }

        public async Task<Reminder?> UpdateReminderAsync(Reminder reminder)
        {
            var existingReminder = await _context.Reminder.FindAsync(reminder.ReminderId);  // Find the reminder by ID
            if (existingReminder == null)
            {
                return null;  // If the reminder doesn't exist, return null
            }

            // Update the reminder with new values
            _context.Entry(existingReminder).CurrentValues.SetValues(reminder);
            await _context.SaveChangesAsync();  // Save changes to the database
            return existingReminder;  // Return the updated reminder
        }
    }
}
