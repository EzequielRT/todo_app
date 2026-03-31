using Todo.Domain.Enums;

namespace Todo.Application.TodoItems.DTOs;

public record TodoItemResponse(
    Guid Id,
    string Title,
    string? Description,
    TodoItemStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
