using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly CatalogContext _dbContext;

    public EfRepository(CatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<T?> GetBySpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).CountAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
    }
}
