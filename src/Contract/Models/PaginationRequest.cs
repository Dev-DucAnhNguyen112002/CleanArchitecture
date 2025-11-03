using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitectureTest.Contract.Models
{
    public abstract record PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Keyword { get; set; }
        public string? SortBy { get; set; }
        public bool? IsDesc { get; set; }
        public PaginationRequest()
        {
            PageNumber = 1;
            PageSize = 20;
        }
        public PaginationRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize;
        }
    }
}
