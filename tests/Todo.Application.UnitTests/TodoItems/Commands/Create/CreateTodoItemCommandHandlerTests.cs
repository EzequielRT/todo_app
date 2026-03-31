using Moq;
using Todo.Application.Common.Interfaces;
using Todo.Application.TodoItems.Commands.Create;
using Todo.Domain.Entities;
using Xunit;

namespace Todo.Application.UnitTests.TodoItems.Commands.Create;

public class CreateTodoItemCommandHandlerTests
{
    private readonly Mock<ITodoItemRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateTodoItemCommandHandler _handler;

    public CreateTodoItemCommandHandlerTests()
    {
        _mockRepository = new Mock<ITodoItemRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateTodoItemCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var command = new CreateTodoItemCommand("Test Task", "Desc", DateTime.UtcNow.AddDays(1), "user123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Title, result.Value!.Title);
        _mockRepository.Verify(x => x.Add(It.IsAny<TodoItem>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
