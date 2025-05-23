using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories.ManagerRepositories;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Data.Repositories.ManagerRepositories
{
    public class ManagerNotificationRepository : IManagerNotificationRepository
    {
        private readonly AppDbContext _context;

        public ManagerNotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications.ToListAsync().ConfigureAwait(false);
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification).ConfigureAwait(false);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }
    }
}