using SoftPlus.Todo.Interfaces.DTOs.Categories;
using SoftPlus.Todo.Interfaces.Entities;
using SoftPlus.Todo.Interfaces.Repositories;
using SoftPlus.Todo.Interfaces.Services;

namespace SoftPlus.Todo.Services.Services;

public sealed class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<List<CategoryDto>> GetAllForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

        return [.. categories.Select(c => new CategoryDto(c.Id, c.Name))];
    }

    public async Task<CategoryDto> CreateAsync(int userId, CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var category = new Category
        {
            Name = dto.Name,
            UserId = userId
        };

        await categoryRepository.AddAsync(category, cancellationToken).ConfigureAwait(false);

        return new CategoryDto(category.Id, category.Name);
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken).ConfigureAwait(false)
                    ?? throw new UnauthorizedAccessException("Категорію не знайдено або спроба видалити чужу категорію.");

        await categoryRepository.DeleteAsync(category, cancellationToken).ConfigureAwait(false);

        return true;
    }
}