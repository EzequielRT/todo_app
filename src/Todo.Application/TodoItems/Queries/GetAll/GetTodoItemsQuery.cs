using MediatR;
using Todo.Application.Common.Interfaces;
using Todo.Application.Common.Models;
using Todo.Application.TodoItems.DTOs;
using Todo.Domain.Enums;

namespace Todo.Application.TodoItems.Queries.GetAll;

public record GetTodoItemsQuery(
    string UserId,
    TodoItemStatus? Status = null,
    DateTime? DueBefore = null,
    DateTime? DueAfter = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedList<TodoItemResponse>>>;

public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, Result<PagedList<TodoItemResponse>>>
{
    private readonly ITodoItemRepository _repository;

    public GetTodoItemsQueryHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedList<TodoItemResponse>>> Handle(GetTodoItemsQuery request, CancellationToken ct)
    {
        var (items, totalCount) = await _repository.GetAllAsync(
            request.UserId,
            request.Status,
            request.DueBefore,
            request.DueAfter,
            request.PageNumber,
            request.PageSize,
            ct);

        var responses = items.Select(item => new TodoItemResponse(
            item.Id,
            item.Title,
            item.Description,
            item.Status,
            item.DueDate,
            item.CreatedAt,
            item.UpdatedAt)).ToList();

        return new PagedList<TodoItemResponse>(
            responses,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
