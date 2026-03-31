using MediatR;
using Todo.Application.Common.Interfaces;
using Todo.Application.Common.Models;
using Todo.Domain.Enums;

namespace Todo.Application.TodoItems.Commands.Update;

public record UpdateTodoItemCommand(
    Guid Id,
    string Title,
    string? Description,
    TodoItemStatus Status,
    DateTime? DueDate,
    string UserId) : IRequest<Result>;

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, Result>
{
    private readonly ITodoItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTodoItemCommandHandler(ITodoItemRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTodoItemCommand request, CancellationToken ct)
    {
        var item = await _repository.GetByIdAsync(request.Id, request.UserId, ct);

        if (item == null)
        {
            return Result.Failure(ResultError.NotFound($"Todo item with ID {request.Id} was not found."));
        }

        item.Update(
            request.Title,
            request.Description,
            request.Status,
            request.DueDate);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
