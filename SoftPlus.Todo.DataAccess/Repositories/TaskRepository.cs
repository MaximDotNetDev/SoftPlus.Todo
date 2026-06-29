using Microsoft.EntityFrameworkCore;
using SoftPlus.Todo.Interfaces.Entities;
using SoftPlus.Todo.Interfaces.Pagination;
using SoftPlus.Todo.Interfaces.Repositories;

namespace SoftPlus.Todo.DataAccess.Repositories;

public sealed class TaskRepository(AppDbContext context) : ITaskRepository
{
    public async Task<PagedResult<TodoTask>> GetPagedTasksAsync(
        int userId,
        int pageNumber,
        int pageSize,
        string? searchTerm,
        int? categoryId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TodoTask> query = context.Tasks
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => t.Title.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

        int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        List<TodoTask> items = await query
            .OrderByDescending(t => t.DueDate.HasValue)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<TodoTask>(items, totalCount, pageNumber, pageSize);
    }

    public Task<TodoTask?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        return context.Tasks
            .Include(t => t.Category)
            .Include(t => t.SubSteps)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        context.Tasks.Add(task);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        context.Tasks.Update(task);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        context.Tasks.Remove(task);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}