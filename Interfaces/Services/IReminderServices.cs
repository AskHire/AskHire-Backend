using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public interface IReminderService
    {
        Task<Reminder> CreateReminderAsync(Reminder reminder);
        Task<Reminder?> GetReminderByIdAsync(Guid id);
        Task<IEnumerable<Reminder>> GetAllRemindersAsync();
        Task<Reminder?> UpdateReminderAsync(Reminder reminder);
        Task<bool> DeleteReminderAsync(Guid id);
    }

}