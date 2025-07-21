using System.Text;

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
        var sb = new StringBuilder();

        sb.Append(nameof(PageNumber)).Append('=').Append(PageNumber).Append(", ");
        sb.Append(nameof(PageSize)).Append('=').Append(PageSize).Append(", ");
        sb.Append(nameof(Search)).Append("=\"").Append(Search ?? "null").Append("\", ");
        sb.Append(nameof(SortBy)).Append("=\"").Append(SortBy ?? "null").Append("\", ");
        sb.Append(nameof(SortDescending)).Append('=').Append(SortDescending);

        return sb.ToString();
    }
}