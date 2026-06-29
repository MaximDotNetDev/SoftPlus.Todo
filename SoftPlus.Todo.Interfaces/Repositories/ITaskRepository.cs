using SoftPlus.Todo.Interfaces.Entities;
using SoftPlus.Todo.Interfaces.Pagination;

namespace SoftPlus.Todo.Interfaces.Repositories;

public interface ITaskRepository
{
   public Task<PagedResult<TodoTask>> GetPagedTasksAsync(
        int userId,
        int pageNumber,
        int pageSize,
        string? searchTerm,
        int? categoryId,
        CancellationToken cancellationToken = default);

    public Task<TodoTask?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    public Task AddAsync(TodoTask task, CancellationToken cancellationToken = default);
    public Task UpdateAsync(TodoTask task, CancellationToken cancellationToken = default);
    public Task DeleteAsync(TodoTask task, CancellationToken cancellationToken = default);
}