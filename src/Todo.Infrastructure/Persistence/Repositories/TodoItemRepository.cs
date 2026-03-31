using Microsoft.EntityFrameworkCore;
using Todo.Application.Common.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Infrastructure.Persistence.Repositories;

public class TodoItemRepository : ITodoItemRepository
{
    private readonly ApplicationDbContext _context;

    public TodoItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default)
    {
        return await _context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);
    }

    public async Task<(List<TodoItem> Items, int TotalCount)> GetAllAsync(
        string userId,
        TodoItemStatus? status = null,
        DateTime? dueBefore = null,
        DateTime? dueAfter = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = _context.TodoItems
            .Where(x => x.UserId == userId);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (dueBefore.HasValue)
            query = query.Where(x => x.DueDate <= dueBefore.Value);

        if (dueAfter.HasValue)
            query = query.Where(x => x.DueDate >= dueAfter.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public void Add(TodoItem item)
    {
        _context.TodoItems.Add(item);
    }

    public void Remove(TodoItem item)
    {
        _context.TodoItems.Remove(item);
    }
}
