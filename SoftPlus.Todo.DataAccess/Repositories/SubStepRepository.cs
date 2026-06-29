using Microsoft.EntityFrameworkCore;
using SoftPlus.Todo.Interfaces.Entities;
using SoftPlus.Todo.Interfaces.Repositories;

namespace SoftPlus.Todo.DataAccess.Repositories;

public sealed class SubStepRepository(AppDbContext context) : ISubStepRepository
{
    public Task<SubStep?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        return context.SubSteps
            .Include(s => s.Task)
            .FirstOrDefaultAsync(s => s.Id == id && s.Task!.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(SubStep subStep, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(subStep);

        context.SubSteps.Add(subStep);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(SubStep subStep, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(subStep);

        context.SubSteps.Update(subStep);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(SubStep subStep, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(subStep);

        context.SubSteps.Remove(subStep);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}