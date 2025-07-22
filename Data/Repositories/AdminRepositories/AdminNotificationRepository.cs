using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class AdminNotificationRepository : IAdminNotificationRepository
{
    private readonly AppDbContext _context;

    public AdminNotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetAllAsync()
    {
        return await _context.Notifications
            .OrderByDescending(n => n.Time)
            .ToListAsync();
    }

    public Task<Notification?> GetByIdAsync(Guid id) =>
        _context.Notifications.FindAsync(id).AsTask();

    public async Task<Notification> CreateAsync(Notification notification)
    {
        // Last‑chance fallback (in case upstream missed)
        notification.Status ??= "Admin";
        if (notification.Time == default) notification.Time = DateTime.Now;

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null) return false;

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PaginatedResult<Notification>> GetPagedAsync(PaginationQuery query)
    {
        var notificationsQuery = _context.Notifications.AsQueryable();

        // Search by Subject (case‑insensitive)
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            var lowerSearch = query.SearchTerm.ToLower();
            notificationsQuery = notificationsQuery.Where(n =>
                n.Subject.ToLower().Contains(lowerSearch));
        }

        // Sorting
        if (!string.IsNullOrEmpty(query.SortBy))
        {
            notificationsQuery = query.IsDescending
                ? notificationsQuery.OrderByDescending(n => EF.Property<object>(n, query.SortBy))
                : notificationsQuery.OrderBy(n => EF.Property<object>(n, query.SortBy));
        }
        else
        {
            notificationsQuery = notificationsQuery.OrderByDescending(n => n.Time);
        }

        var totalCount = await notificationsQuery.CountAsync();

        var notifications = await notificationsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PaginatedResult<Notification>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
            Data = notifications
        };
    }
}
