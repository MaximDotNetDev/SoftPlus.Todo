using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoftPlus.Todo.Interfaces.DTOs.Task;
using SoftPlus.Todo.Interfaces.Pagination;
using SoftPlus.Todo.Interfaces.Services;
using SoftPlus.Todo.Api.Extensions;

namespace SoftPlus.Todo.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] TaskFilterDto filter, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await taskService.GetPagedAsync(userId, filter, cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var task = await taskService.GetByIdAsync(id, userId, cancellationToken).ConfigureAwait(false);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var createdTask = await taskService.CreateAsync(userId, dto, cancellationToken).ConfigureAwait(false);

        return Ok(createdTask);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (id != dto.Id)
        {
            return BadRequest("ID у URL не збігається з ID у тілі запиту.");
        }

        var userId = User.GetUserId();
        var success = await taskService.UpdateAsync(userId, dto, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/toggle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleComplete(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var success = await taskService.ToggleCompleteAsync(id, userId, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var success = await taskService.DeleteAsync(id, userId, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}