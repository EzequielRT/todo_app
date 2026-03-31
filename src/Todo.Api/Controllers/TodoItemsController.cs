using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.TodoItems.Commands.Create;
using Todo.Application.TodoItems.Commands.Delete;
using Todo.Application.TodoItems.Commands.Update;
using Todo.Application.TodoItems.Queries.GetAll;
using Todo.Application.TodoItems.Queries.GetById;
using Todo.Application.TodoItems.DTOs;
using Todo.Application.Common.Models;
using Todo.Domain.Enums;

namespace Todo.Api.Controllers;

[ApiVersion("1.0")]
[Authorize]
public class TodoItemsController : ApiController
{
    private readonly ISender _sender;

    public TodoItemsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetTodoItemByIdQuery(id, UserId));
        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<TodoItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] TodoItemStatus? status,
        [FromQuery] DateTime? dueBefore,
        [FromQuery] DateTime? dueAfter,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _sender.Send(new GetTodoItemsQuery(
            UserId, status, dueBefore, dueAfter, pageNumber, pageSize));
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemRequest request)
    {
        var result = await _sender.Send(new CreateTodoItemCommand(
            request.Title, request.Description, request.DueDate, UserId));

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
        }

        return HandleResult(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoItemRequest request)
    {
        var result = await _sender.Send(new UpdateTodoItemCommand(
            id, request.Title, request.Description, request.Status, request.DueDate, UserId));

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _sender.Send(new DeleteTodoItemCommand(id, UserId));

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleResult(result);
    }
}

public record CreateTodoItemRequest(string Title, string? Description, DateTime? DueDate);
public record UpdateTodoItemRequest(string Title, string? Description, TodoItemStatus Status, DateTime? DueDate);
