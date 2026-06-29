using SoftPlus.Todo.Interfaces.Entities;

namespace SoftPlus.Todo.Interfaces.Repositories;

public interface ISubStepRepository
{
    public Task<SubStep?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    public Task AddAsync(SubStep subStep, CancellationToken cancellationToken = default);
    public Task UpdateAsync(SubStep subStep, CancellationToken cancellationToken = default);
    public Task DeleteAsync(SubStep subStep, CancellationToken cancellationToken = default);
}