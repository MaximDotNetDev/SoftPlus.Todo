using SoftPlus.Todo.Interfaces.DTOs.Task;
using SoftPlus.Todo.Interfaces.Pagination;

namespace SoftPlus.Todo.Interfaces.Services;

public interface ITaskService
{
    public Task<PagedResult<TaskDto>> GetPagedAsync(int userId, TaskFilterDto filter, CancellationToken cancellationToken = default);
    public Task<TaskDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    public Task<TaskDto> CreateAsync(int userId, CreateTaskDto dto, CancellationToken cancellationToken = default);
    public Task<bool> UpdateAsync(int userId, UpdateTaskDto dto, CancellationToken cancellationToken = default);
    public Task<bool> ToggleCompleteAsync(int id, int userId, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}