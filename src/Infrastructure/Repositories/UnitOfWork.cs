using CleanArchitectureTest.Application.Interfaces;
using CleanArchitectureTest.Application.Interfaces.Repositories;
using CleanArchitectureTest.Infrastructure.Data;
using CleanArchitectureTest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitectureTest.Infrastructure.Repositories;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();

    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IGenericRepository<T>)_repositories[typeof(T)];
        }

        var repository = new GenericRepository<T>(dbContext);
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    public int ExecuteSqlCommand(string sql, params object[] parameters) => dbContext.Database.ExecuteSqlRaw(sql, parameters);

    public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters) => await dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

    public IQueryable<T> SqlQueryRaw<T>(string sql, params object[] parameters) where T : class => dbContext.Database.SqlQueryRaw<T>(sql, parameters).AsQueryable();

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
    public bool SaveChanges()
    {
        return dbContext.SaveChanges() > 0;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => dbContext.Database.BeginTransactionAsync(cancellationToken);

    public Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        => transaction.CommitAsync(cancellationToken);

    public Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        => transaction.RollbackAsync(cancellationToken);

    public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }
}
