using SoftPlus.Todo.Interfaces.DTOs.SubSteps;
using SoftPlus.Todo.Interfaces.Entities;
using SoftPlus.Todo.Interfaces.Repositories;
using SoftPlus.Todo.Interfaces.Services;

namespace SoftPlus.Todo.Services.Services;

public sealed class SubStepService(
    ITaskRepository taskRepository,
    ISubStepRepository subStepRepository) : ISubStepService
{
    public async Task<SubStepDto?> CreateAsync(int taskId, int userId, CreateSubStepDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var task = await taskRepository.GetByIdAndUserIdAsync(taskId, userId, cancellationToken).ConfigureAwait(false);
        if (task == null)
        {
            return null;
        }

        var subStep = new SubStep
        {
            Title = dto.Title,
            IsCompleted = false,
            TaskId = taskId
        };

        await subStepRepository.AddAsync(subStep, cancellationToken).ConfigureAwait(false);

        return new SubStepDto(subStep.Id, subStep.Title, subStep.IsCompleted, subStep.TaskId);
    }

    public async Task<bool> UpdateAsync(int id, int taskId, int userId, UpdateSubStepDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var subStep = await subStepRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken).ConfigureAwait(false);
        if (subStep == null || subStep.TaskId != taskId)
        {
            return false;
        }

        subStep.Title = dto.Title;
        subStep.IsCompleted = dto.IsCompleted;

        await subStepRepository.UpdateAsync(subStep, cancellationToken).ConfigureAwait(false);

        return true;
    }

    public async Task<bool> ToggleCompleteAsync(int id, int taskId, int userId, CancellationToken cancellationToken = default)
    {
        var subStep = await subStepRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken).ConfigureAwait(false);
        if (subStep == null || subStep.TaskId != taskId)
        {
            return false;
        }

        subStep.IsCompleted = !subStep.IsCompleted;

        await subStepRepository.UpdateAsync(subStep, cancellationToken).ConfigureAwait(false);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, int taskId, int userId, CancellationToken cancellationToken = default)
    {
        var subStep = await subStepRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken).ConfigureAwait(false);
        if (subStep == null || subStep.TaskId != taskId)
        {
            return false;
        }

        await subStepRepository.DeleteAsync(subStep, cancellationToken).ConfigureAwait(false);

        return true;
    }
}