using MediatR;
using Todo.Application.Common.Interfaces;
using Todo.Application.Common.Models;
using Todo.Application.TodoItems.DTOs;

namespace Todo.Application.TodoItems.Queries.GetById;

public record GetTodoItemByIdQuery(Guid Id, string UserId) : IRequest<Result<TodoItemResponse>>;

public class GetTodoItemByIdQueryHandler : IRequestHandler<GetTodoItemByIdQuery, Result<TodoItemResponse>>
{
    private readonly ITodoItemRepository _repository;

    public GetTodoItemByIdQueryHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TodoItemResponse>> Handle(GetTodoItemByIdQuery request, CancellationToken ct)
    {
        var item = await _repository.GetByIdAsync(request.Id, request.UserId, ct);

        if (item == null)
        {
            return ResultError.NotFound($"Todo item with ID {request.Id} was not found.");
        }

        return new TodoItemResponse(
            item.Id,
            item.Title,
            item.Description,
            item.Status,
            item.DueDate,
            item.CreatedAt,
            item.UpdatedAt);
    }
}
