using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AskHire_Backend.Interfaces.Repositories
{
    public interface IReminderRepository
    {
        Task<Reminder> CreateReminderAsync(Reminder reminder);
        Task<Reminder?> GetReminderByIdAsync(Guid id);
        Task<IEnumerable<Reminder>> GetAllRemindersAsync();
        Task<Reminder?> UpdateReminderAsync(Reminder reminder);
        Task<bool> DeleteReminderAsync(Guid id);
    }
}
