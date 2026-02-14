namespace CleanStart.Domain.Repositories.Base;
public class PagedListQuery
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public OrderDirection OrderDirection { get; set; }
}

public class PagedListQuery<T> : PagedListQuery where T : class
{
    public T? Filter { get; set; }
}

public enum OrderDirection
{
    Asc = 1,
    Desc = 2
}
