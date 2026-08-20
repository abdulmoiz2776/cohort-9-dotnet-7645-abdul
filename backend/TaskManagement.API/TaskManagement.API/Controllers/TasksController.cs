using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Task.AssignTask;
using TaskManagement.Application.Features.Task.ChangeStatus;
using TaskManagement.Application.Features.Task.CreateTask;
using TaskManagement.Application.Features.Task.DeleteTask;
using TaskManagement.Application.Features.Task.GetTaskById;
using TaskManagement.Application.Features.Task.GetTasks;
using TaskManagement.Application.Features.Task.UpdateTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var result = await _mediator.Send(new DeleteTaskCommand { Id = id });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> AssignTask(Guid id, AssignTaskRequest request)
    {
        var command = new AssignTaskCommand
        {
            Id = id,
            AssignedToUserId = request.AssignedToUserId
        };

        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> ChangeTaskStatus(Guid id, ChangeTaskStatusRequest request)
    {
        var command = new ChangeTaskStatusCommand
        {
            Id = id,
            Status = request.Status
        };

        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    public sealed class AssignTaskRequest
    {
        public string AssignedToUserId { get; set; } = string.Empty;
    }

    public sealed class ChangeTaskStatusRequest
    {
        public TaskStatus Status { get; set; }
    }
}
