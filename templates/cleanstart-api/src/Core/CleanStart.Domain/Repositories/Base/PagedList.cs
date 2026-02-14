namespace CleanStart.Domain.Repositories.Base;
public class PagedList<T> where T : class
{
    public IEnumerable<T>? Data { get; set; }
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
