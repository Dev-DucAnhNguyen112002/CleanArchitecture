using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanArchitectureTest.Contract.Models;

public class PagedResult<T> : BaseResult<List<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasNextPage => PageNumber * PageSize < TotalItems;
    public bool HasPreviousPage => PageNumber > 1;

    public PagedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalItems, int? totalNestedItems = null)
    {
        Success = true;
        Data = data.ToList();
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = pageSize == -1 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
    }

    public PagedResult() { }

    public new static PagedResult<T> Failure(Error error)
        => new() { Success = false, Errors = [error] };

    public new static PagedResult<T> Failure(IEnumerable<Error> errors)
        => new() { Success = false, Errors = errors.ToList() };


    public static implicit operator PagedResult<T>(Error error)
        => new() { Success = false, Errors = [error] };

    public static implicit operator PagedResult<T>(List<Error> errors)
        => new() { Success = false, Errors = errors };
}
