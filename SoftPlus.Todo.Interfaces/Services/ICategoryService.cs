using SoftPlus.Todo.Interfaces.DTOs.Categories;

namespace SoftPlus.Todo.Interfaces.Services;

public interface ICategoryService
{
    public Task<List<CategoryDto>> GetAllForUserAsync(int userId, CancellationToken cancellationToken = default);
    public Task<CategoryDto> CreateAsync(int userId, CreateCategoryDto dto, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}