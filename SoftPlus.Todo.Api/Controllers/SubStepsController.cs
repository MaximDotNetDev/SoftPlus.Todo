using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoftPlus.Todo.Interfaces.DTOs.SubSteps;
using SoftPlus.Todo.Interfaces.Services;
using SoftPlus.Todo.Api.Extensions;

namespace SoftPlus.Todo.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks/{taskId:int}/[controller]")]
public sealed class SubStepsController(ISubStepService subStepService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(SubStepDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int taskId, [FromBody] CreateSubStepDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await subStepService.CreateAsync(taskId, userId, dto, cancellationToken).ConfigureAwait(false);

        if (result is null)
        {
            return NotFound("Базове завдання не знайдено.");
        }

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int taskId, int id, [FromBody] UpdateSubStepDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (id != dto.Id)
        {
            return BadRequest("ID у шлюзі не збігається з ID у тілі.");
        }

        var userId = User.GetUserId();
        var success = await subStepService.UpdateAsync(id, taskId, userId, dto, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/toggle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Toggle(int taskId, int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var success = await subStepService.ToggleCompleteAsync(id, taskId, userId, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int taskId, int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var success = await subStepService.DeleteAsync(id, taskId, userId, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}