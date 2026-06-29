using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoftPlus.Todo.Interfaces.DTOs.Categories;
using SoftPlus.Todo.Interfaces.Services;
using SoftPlus.Todo.Api.Extensions;

namespace SoftPlus.Todo.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var categories = await categoryService.GetAllForUserAsync(userId, cancellationToken).ConfigureAwait(false);

        return Ok(categories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var category = await categoryService.CreateAsync(userId, dto, cancellationToken).ConfigureAwait(false);

        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var success = await categoryService.DeleteAsync(id, userId, cancellationToken).ConfigureAwait(false);

        if (!success)
        {
            return NotFound("Категорію не знайдено або доступ заборонено.");
        }

        return NoContent();
    }
}