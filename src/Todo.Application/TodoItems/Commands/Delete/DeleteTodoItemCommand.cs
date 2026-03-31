using MediatR;
using Todo.Application.Common.Interfaces;
using Todo.Application.Common.Models;

namespace Todo.Application.TodoItems.Commands.Delete;

public record DeleteTodoItemCommand(Guid Id, string UserId) : IRequest<Result>;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, Result>
{
    private readonly ITodoItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTodoItemCommandHandler(ITodoItemRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteTodoItemCommand request, CancellationToken ct)
    {
        var item = await _repository.GetByIdAsync(request.Id, request.UserId, ct);

        if (item == null)
        {
            return Result.Failure(ResultError.NotFound($"Todo item with ID {request.Id} was not found."));
        }

        _repository.Remove(item);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
