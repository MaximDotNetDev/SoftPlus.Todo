using Microsoft.EntityFrameworkCore;
using SoftPlus.Todo.Interfaces.Entities;
using SoftPlus.Todo.Interfaces.Repositories;

namespace SoftPlus.Todo.DataAccess.Repositories;

public sealed class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public Task<List<Category>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAndUserIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        return context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);

        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);

        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}