using SoftPlus.Todo.Interfaces.Entities;

namespace SoftPlus.Todo.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    public Task AddAsync(User user, CancellationToken cancellationToken = default);
}