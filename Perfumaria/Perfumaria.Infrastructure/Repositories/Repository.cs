using Perfumaria.Application.Interfaces.Repositories;
using Perfumaria.Domain.Common;
using Perfumaria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Perfumaria.Infrastructure.Repositories;

/// <summary>
/// Implementação EF Core do repositório genérico. Usa <c>Set&lt;TEntity&gt;()</c> do
/// <see cref="PerfumariaDbContext"/> e aplica <c>AsNoTracking</c> nas leituras.
/// </summary>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    protected readonly PerfumariaDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(PerfumariaDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> ObterPorIdAsync(TKey id)
        => await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<TEntity>> ObterTodosAsync()
        => await _dbSet.AsNoTracking().ToListAsync();

    public virtual async Task AdicionarAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task AtualizarAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task RemoverAsync(TKey id)
    {
        var entity = await ObterPorIdAsync(id);
        if (entity is null) return;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<bool> ExisteAsync(TKey id)
        => await _dbSet.AsNoTracking().AnyAsync(e => e.Id!.Equals(id));
}
