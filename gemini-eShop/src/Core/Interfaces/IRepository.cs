using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces;

public interface IRepository<T> : IReadRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
