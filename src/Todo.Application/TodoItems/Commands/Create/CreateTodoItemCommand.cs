using MediatR;
using Todo.Application.Common.Interfaces;
using Todo.Application.Common.Models;
using Todo.Application.TodoItems.DTOs;
using Todo.Domain.Entities;

namespace Todo.Application.TodoItems.Commands.Create;

public record CreateTodoItemCommand(
    string Title,
    string? Description,
    DateTime? DueDate,
    string UserId) : IRequest<Result<TodoItemResponse>>;

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Result<TodoItemResponse>>
{
    private readonly ITodoItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTodoItemCommandHandler(ITodoItemRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TodoItemResponse>> Handle(CreateTodoItemCommand request, CancellationToken ct)
    {
        var item = TodoItem.Create(
            request.Title,
            request.Description,
            request.DueDate,
            request.UserId);

        _repository.Add(item);
        await _unitOfWork.SaveChangesAsync(ct);

        var response = new TodoItemResponse(
            item.Id,
            item.Title,
            item.Description,
            item.Status,
            item.DueDate,
            item.CreatedAt,
            item.UpdatedAt);

        return response; // Implicit conversion from T to Result<T>
    }
}
