namespace Application.Models.PaginationModels;

public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;

    public override string ToString()
    {
        return $@"{nameof(PageNumber)}={PageNumber}, {nameof(PageSize)}={PageSize}, {nameof(Search)}=""{Search ?? "null"}"", {nameof(SortBy)}=""{SortBy ?? "null"}"", {nameof(SortDescending)}={SortDescending}";
    }
}