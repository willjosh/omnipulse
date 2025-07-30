namespace Application.Models.PaginationModels;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];

    /// <example>1</example>
    public int TotalCount { get; set; }

    /// <example>1</example>
    public int PageNumber { get; set; }

    /// <example>50</example>
    public int PageSize { get; set; }

    /// <example>1</example>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <example>false</example>
    public bool HasPreviousPage => PageNumber > 1;

    /// <example>false</example>
    public bool HasNextPage => PageNumber < TotalPages;
}