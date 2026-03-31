namespace Todo.Application.Common.Models;

public record PagedList<T>(
    List<T> Items, 
    int PageNumber, 
    int PageSize, 
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
