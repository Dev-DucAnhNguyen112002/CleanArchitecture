using CleanArchitectureTest.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitectureTest.Application.Interfaces;
public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveChangesAsync();

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);

    IGenericRepository<T> Repository<T>() where T : class;

    int ExecuteSqlCommand(string sql, params object[] parameters);

    Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

    IQueryable<T> SqlQueryRaw<T>(string sql, params object[] parameters) where T : class;
}
