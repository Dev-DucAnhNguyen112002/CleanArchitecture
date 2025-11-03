using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitectureTest.Application.Common.Extentions;
using CleanArchitectureTest.Application.Helpers;
using CleanArchitectureTest.Application.Interfaces.Repositories;
using CleanArchitectureTest.Contract.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CleanArchitectureTest.Infrastructure.Repositories;

public class GenericRepository<T>(DbContext dbContext) : IGenericRepository<T> where T : class
{
    public virtual async Task<T?> GetByIdAsync(object id)
    {
        return await dbContext.Set<T>().FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public void Update(T entity)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }

    public void SoftDeleteRange(IEnumerable<T> entities, string? username = null)
    {
        foreach (var entity in entities)
        {
            dbContext.Entry(entity).Property(EntityPropertyNames.IsDeleted).CurrentValue = true;
            dbContext.Entry(entity).State = EntityState.Modified;
        }
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await dbContext
            .Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool disableTracking = true)
    {
        IQueryable<T> query = dbContext.Set<T>();
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.FirstOrDefaultAsync();
    }

    public TResult? GetFirstOrDefault<TResult>(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool disableTracking = true,
        IMapper? mapper = null)
    {
        IQueryable<T> query = dbContext.Set<T>();

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var entity = query.FirstOrDefault();

        if (mapper != null)
        {
            return mapper.Map<TResult>(entity);
        }

        return entity is TResult result ? result : default;
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        dbContext.Set<T>().RemoveRange(entities);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await dbContext.Set<T>().AddRangeAsync(entities);
    }



    public void UpdateRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
        }
    }

    public async Task<bool> ExistsByPropertyAsync<TType>(
        string propertyName,
        string value,
        string? idFieldName = null,
        TType? idValue = default)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(value))
            return false;

        var query = dbContext.Set<T>()
            .AsNoTracking()
            .Where(BuildPropertyFilter(propertyName, value));

        // Loại trừ record hiện tại nếu đang update
        if (!string.IsNullOrWhiteSpace(idFieldName) && idValue != null && !Equals(idValue, default(TType)))
        {
            query = query.Where(BuildExcludeFilter<TType>(idFieldName, idValue));
        }

        return await query.AnyAsync();
    }

    private Expression<Func<T, bool>> BuildPropertyFilter(string propertyName, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var valueConstant = Expression.Constant(value);
        var equals = Expression.Equal(property, valueConstant);

        return Expression.Lambda<Func<T, bool>>(equals, parameter);
    }

    private Expression<Func<T, bool>> BuildExcludeFilter<TType>(string idFieldName, TType? idValue)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var idProperty = Expression.Property(parameter, idFieldName);
        var idConstant = Expression.Constant(idValue, typeof(TType));
        var notEquals = Expression.NotEqual(idProperty, idConstant);

        return Expression.Lambda<Func<T, bool>>(notEquals, parameter);
    }

    public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool disableTracking = true)
    {
        IQueryable<T> query = dbContext.Set<T>();
        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }

    public async Task<PagedResult<TResult>> GetPagedListAsync<TResult>(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        IMapper? mapper = null,
        int pageIndex = 1,
        int pageSize = 20,
        int rowModify = 0,
        bool disableTracking = true)
    {
        var query = GetQueryable(filter, orderBy, include, disableTracking);

        if (mapper != null)
            return await query.ProjectTo<TResult>(mapper.ConfigurationProvider)
                .ToPagedListAsync(pageIndex, pageSize, rowModify);

        return await query.Project().To<TResult>()
            .ToPagedListAsync(pageIndex: pageIndex, pageSize: pageSize, rowModify: rowModify);
    }

    public IQueryable<TEntity> ApplyOrdering<TEntity>(IQueryable<TEntity> query, string? sortBy, bool? isDesc)
        where TEntity : class
    {
        if (string.IsNullOrEmpty(sortBy)) return query;

        var orderParams = sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries);

        IOrderedQueryable<TEntity>? orderedQuery = null;

        foreach (var param in orderParams)
        {
            // Convert property name to PascalCase
            var words = param.Split('_', ' ');
            var propertyName = string.Join("", words.Select(w => char.ToUpper(w[0]) + w.Substring(1)));

            if (orderedQuery == null)
            {
                orderedQuery = isDesc == true
                    ? query.OrderByDescending(e => EF.Property<object>(e, propertyName))
                    : query.OrderBy(e => EF.Property<object>(e, propertyName));
            }
            else
            {
                orderedQuery = isDesc == true
                    ? orderedQuery.ThenByDescending(e => EF.Property<object>(e, propertyName))
                    : orderedQuery.ThenBy(e => EF.Property<object>(e, propertyName));
            }
        }

        return orderedQuery ?? query;
    }

    public async Task<PagedResult<TResult>> GetPagedListAsync<TResult>(IQueryable<T> query, IMapper? mapper = null, int pageIndex = 1, int pageSize = 20, int rowModify = 0, bool disableTracking = true)
    {
        if (mapper != null)
            return await query.ProjectTo<TResult>(mapper.ConfigurationProvider)
                .ToPagedListAsync(pageIndex, pageSize, rowModify);

        return await query.Project().To<TResult>()
            .ToPagedListAsync(pageIndex: pageIndex, pageSize: pageSize, rowModify: rowModify);
    }

    public async Task<List<T>> GetByIdsAsync<TKey>(IEnumerable<TKey> ids, Expression<Func<T, TKey>> keySelector)
    {
        if (ids == null || !ids.Any())
            return [];

        // Get property name from expression
        var member = (keySelector.Body as MemberExpression) ??
                     ((UnaryExpression)keySelector.Body).Operand as MemberExpression;
        if (member == null)
            throw new ArgumentException("Invalid expression");

        var key = member.Member.Name;

        // Batch processing to avoid SQL parameter limits
        const int batchSize = 1000;
        var result = new List<T>();

        foreach (var batch in ids.Chunk(batchSize))
        {
            var batchResult = await dbContext.Set<T>()
                .Where(e => batch.Contains(EF.Property<TKey>(e, key)))
                .ToListAsync();
            result.AddRange(batchResult);
        }

        return result;
    }
}
