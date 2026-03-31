using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Common.Interfaces;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default);
    Task<(List<TodoItem> Items, int TotalCount)> GetAllAsync(
        string userId,
        TodoItemStatus? status = null,
        DateTime? dueBefore = null,
        DateTime? dueAfter = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken ct = default);
    
    void Add(TodoItem item);
    void Remove(TodoItem item);
}
