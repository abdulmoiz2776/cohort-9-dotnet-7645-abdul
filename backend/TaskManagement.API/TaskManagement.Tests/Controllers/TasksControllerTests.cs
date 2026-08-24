using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Task.AssignTask;
using TaskManagement.Application.Features.Task.ChangeStatus;
using TaskManagement.Application.Features.Task.Common;
using TaskManagement.Application.Features.Task.CreateTask;
using TaskManagement.Application.Features.Task.DeleteTask;
using TaskManagement.Application.Features.Task.GetTaskById;
using TaskManagement.Application.Features.Task.GetTasks;
using TaskManagement.Application.Features.Task.UpdateTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;
using Xunit;

namespace TaskManagement.Tests.Controllers;

public sealed class TasksControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _controller = new TasksController(_mediator.Object);
    }

    [Fact]
    public async Task GetTasks_Should_Send_Query_And_Return_Ok()
    {
        var query = new GetTasksQuery { SearchTerm = "release", Page = 2 };
        var result = new PagedResult<TaskDto> { Page = 2, PageSize = 10, TotalCount = 1 };
        _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var actionResult = await _controller.GetTasks(query);

        actionResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(result);
        _mediator.Verify(x => x.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTaskById_Should_Send_Query_With_Id_And_Return_Ok()
    {
        var id = Guid.NewGuid();
        var result = new TaskDetailsDto { Id = id };
        _mediator.Setup(x => x.Send(It.Is<GetTaskByIdQuery>(query => query.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var actionResult = await _controller.GetTaskById(id);

        actionResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(result);
    }

    [Fact]
    public async Task CreateTask_Should_Return_Created_At_Task_Details()
    {
        var command = new CreateTaskCommand("Release", "Deploy service", default, null, null, null);
        var result = new CreateTaskResponse { Id = Guid.NewGuid(), Title = command.Title };
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var actionResult = await _controller.CreateTask(command);

        var created = actionResult.Should().BeOfType<CreatedAtActionResult>().Which;
        created.ActionName.Should().Be(nameof(TasksController.GetTaskById));
        created.RouteValues!["id"].Should().Be(result.Id);
        created.Value.Should().Be(result);
    }

    [Fact]
    public async Task UpdateTask_Should_Set_Route_Id_And_Return_Ok_When_Succeeded()
    {
        var id = Guid.NewGuid();
        var command = new UpdateTaskCommand { Title = "Updated" };
        var result = new UpdateTaskResponse { Succeeded = true };
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var actionResult = await _controller.UpdateTask(id, command);

        actionResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(result);
        command.Id.Should().Be(id);
    }

    [Fact]
    public async Task UpdateTask_Should_Return_Bad_Request_When_Unsuccessful()
    {
        var result = new UpdateTaskResponse { Succeeded = false, Message = "Task not found." };
        _mediator.Setup(x => x.Send(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var actionResult = await _controller.UpdateTask(Guid.NewGuid(), new UpdateTaskCommand());

        actionResult.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(result);
    }

    [Fact]
    public async Task DeleteTask_Should_Return_Bad_Request_When_Unsuccessful()
    {
        var id = Guid.NewGuid();
        var result = new DeleteTaskResponse { Succeeded = false, Message = "Task not found." };
        _mediator.Setup(x => x.Send(It.Is<DeleteTaskCommand>(command => command.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var actionResult = await _controller.DeleteTask(id);

        actionResult.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(result);
    }

    [Fact]
    public async Task AssignTask_Should_Map_Request_And_Return_Ok_When_Succeeded()
    {
        var id = Guid.NewGuid();
        var result = new AssignTaskResponse { Succeeded = true };
        _mediator.Setup(x => x.Send(It.IsAny<AssignTaskCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var actionResult = await _controller.AssignTask(id, new TasksController.AssignTaskRequest { AssignedToUserId = "user-1" });

        actionResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(result);
        _mediator.Verify(x => x.Send(It.Is<AssignTaskCommand>(command =>
            command.Id == id && command.AssignedToUserId == "user-1"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangeTaskStatus_Should_Map_Request_And_Return_Bad_Request_When_Unsuccessful()
    {
        var id = Guid.NewGuid();
        var result = new ChangeTaskStatusResponse { Succeeded = false, Message = "Invalid status." };
        _mediator.Setup(x => x.Send(It.IsAny<ChangeTaskStatusCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var actionResult = await _controller.ChangeTaskStatus(
            id,
            new TasksController.ChangeTaskStatusRequest { Status = TaskStatus.Completed });

        actionResult.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(result);
        _mediator.Verify(x => x.Send(It.Is<ChangeTaskStatusCommand>(command =>
            command.Id == id && command.Status == TaskStatus.Completed), It.IsAny<CancellationToken>()), Times.Once);
    }
}