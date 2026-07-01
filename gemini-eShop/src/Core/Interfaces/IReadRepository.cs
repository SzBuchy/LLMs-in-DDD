using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces;

public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T?> GetBySpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
}
