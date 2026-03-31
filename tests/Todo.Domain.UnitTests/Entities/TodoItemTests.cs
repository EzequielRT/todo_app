using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Xunit;

namespace Todo.Domain.UnitTests.Entities;

public class TodoItemTests
{
    [Fact]
    public void Create_WithValidData_ReturnsNewTodoItem()
    {
        // Arrange
        var title = "Test Task";
        var description = "Test Description";
        var userId = "user123";
        var dueDate = DateTime.UtcNow.AddDays(1);

        // Act
        var todoItem = TodoItem.Create(title, description, dueDate, userId);

        // Assert
        Assert.NotEqual(Guid.Empty, todoItem.Id);
        Assert.Equal(title, todoItem.Title);
        Assert.Equal(description, todoItem.Description);
        Assert.Equal(dueDate, todoItem.DueDate);
        Assert.Equal(userId, todoItem.UserId);
        Assert.Equal(TodoItemStatus.Pending, todoItem.Status);
        Assert.True(todoItem.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Create_WithEmptyTitle_ThrowsArgumentException(string title)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => TodoItem.Create(title!, "desc", null, "user123"));
    }

    [Fact]
    public void Update_WithValidData_UpdatesTodoItem()
    {
        // Arrange
        var todoItem = TodoItem.Create("Initial Title", "initial desc", null, "user123");
        var newTitle = "Updated Title";
        var newStatus = TodoItemStatus.InProgress;

        // Act
        todoItem.Update(newTitle, "new desc", newStatus, null);

        // Assert
        Assert.Equal(newTitle, todoItem.Title);
        Assert.Equal(newStatus, todoItem.Status);
        Assert.NotNull(todoItem.UpdatedAt);
    }

    [Fact]
    public void MarkAsCompleted_SetsStatusToCompleted()
    {
        // Arrange
        var todoItem = TodoItem.Create("Title", "desc", null, "user123");

        // Act
        todoItem.MarkAsCompleted();

        // Assert
        Assert.Equal(TodoItemStatus.Completed, todoItem.Status);
        Assert.NotNull(todoItem.UpdatedAt);
    }
}
