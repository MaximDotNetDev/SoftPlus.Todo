using SoftPlus.Todo.Interfaces.DTOs.SubSteps;

namespace SoftPlus.Todo.Interfaces.Services;

public interface ISubStepService
{
    public Task<SubStepDto?> CreateAsync(int taskId, int userId, CreateSubStepDto dto, CancellationToken cancellationToken = default);
    public Task<bool> UpdateAsync(int id, int taskId, int userId, UpdateSubStepDto dto, CancellationToken cancellationToken = default);
    public Task<bool> ToggleCompleteAsync(int id, int taskId, int userId, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(int id, int taskId, int userId, CancellationToken cancellationToken = default);
}