using Todo.Domain.Enums;

namespace Todo.Domain.Entities;

public class TodoItem
{
    private TodoItem(string title, string? description, DateTime? dueDate, string userId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        DueDate = dueDate;
        UserId = userId;
        Status = TodoItemStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TodoItemStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public string UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static TodoItem Create(string title, string? description, DateTime? dueDate, string userId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        }

        return new TodoItem(title, description, dueDate, userId);
    }

    public void Update(string title, string? description, TodoItemStatus status, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        Status = TodoItemStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}
