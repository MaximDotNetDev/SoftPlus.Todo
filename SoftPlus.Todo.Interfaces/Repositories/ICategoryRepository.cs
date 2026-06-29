using SoftPlus.Todo.Interfaces.Entities;

namespace SoftPlus.Todo.Interfaces.Repositories;

public interface ICategoryRepository
{
    public Task<List<Category>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    public Task<Category?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    public Task AddAsync(Category category, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
}