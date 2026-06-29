using SoftPlus.Todo.Interfaces.DTOs.SubSteps;
using SoftPlus.Todo.Interfaces.DTOs.Task;
using SoftPlus.Todo.Interfaces.Entities;
using SoftPlus.Todo.Interfaces.Pagination;
using SoftPlus.Todo.Interfaces.Repositories;
using SoftPlus.Todo.Interfaces.Services;

namespace SoftPlus.Todo.Services.Services;

public sealed class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<PagedResult<TaskDto>> GetPagedAsync(
        int userId,
        TaskFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var pagedResult = await taskRepository.GetPagedTasksAsync(
            userId,
            filter.PageNumber,
            filter.PageSize,
            filter.SearchTerm,
            filter.CategoryId,
            cancellationToken).ConfigureAwait(false);

        List<TaskDto> dtos =
        [
            .. pagedResult.Items.Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.IsCompleted,
                t.DueDate,
                t.CategoryId,
                t.Category?.Name,
                []))
        ];

        return new PagedResult<TaskDto>(dtos, pagedResult.TotalCount, pagedResult.PageNumber, pagedResult.PageSize);
    }

    public async Task<TaskDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken).ConfigureAwait(false);

        if (task == null)
        {
            return null;
        }

        return new TaskDto(
                    task.Id,
                    task.Title,
                    task.Description,
                    task.IsCompleted,
                    task.DueDate,
                    task.CategoryId,
                    task.Category?.Name,
                    [.. task.SubSteps.Select(s => new SubStepDto(s.Id, s.Title, s.IsCompleted, s.TaskId))]);
    }

    public async Task<TaskDto> CreateAsync(int userId, CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var task = new TodoTask
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            CategoryId = dto.CategoryId,
            UserId = userId,
            IsCompleted = false
        };

        await taskRepository.AddAsync(task, cancellationToken).ConfigureAwait(false);

        return new TaskDto(
                    task.Id,
                    task.Title,
                    task.Description,
                    task.IsCompleted,
                    task.DueDate,
                    task.CategoryId,
                    string.Empty,
                    []);
    }

    public async Task<bool> UpdateAsync(int userId, UpdateTaskDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var task = await taskRepository.GetByIdAndUserIdAsync(dto.Id, userId, cancellationToken).ConfigureAwait(false);

        if (task == null)
        {
            return false;
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        task.DueDate = dto.DueDate;
        task.CategoryId = dto.CategoryId;

        await taskRepository.UpdateAsync(task, cancellationToken).ConfigureAwait(false);

        return true;
    }

    public async Task<bool> ToggleCompleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken).ConfigureAwait(false);

        if (task == null)
        {
            return false;
        }

        task.IsCompleted = !task.IsCompleted;

        await taskRepository.UpdateAsync(task, cancellationToken).ConfigureAwait(false);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken).ConfigureAwait(false);

        if (task == null)
        {
            return false;
        }

        await taskRepository.DeleteAsync(task, cancellationToken).ConfigureAwait(false);

        return true;
    }
}