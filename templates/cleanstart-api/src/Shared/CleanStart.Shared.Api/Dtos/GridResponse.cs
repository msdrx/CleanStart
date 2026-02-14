using CleanStart.Domain.Repositories.Base;

namespace CleanStart.Shared.Api.Dtos;

public class GridResponse<T> where T : class
{
    public IEnumerable<T>? Data { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public bool HasPrevious { get { return CurrentPage > 0; } private set { } }
    public bool HasNext { get { return CurrentPage + 1 < TotalPages; } private set { } }

    public static GridResponse<T> MapFrom(PagedList<T> result)
    {
        return new GridResponse<T>()
        {
            Data = result.Data,
            TotalCount = result.TotalCount,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages
        };
    }
}