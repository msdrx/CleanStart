using CleanStart.Domain.Repositories.Base;

namespace CleanStart.Shared.Api.Dtos;

public class GridRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public OrderDirection OrderDirection { get; set; }
    public virtual PagedListQuery ToPaginationQuery()
    {
        return new PagedListQuery()
        {
            OrderDirection = OrderDirection,
            Page = Page,
            PageSize = PageSize
        };
    }
}

public class GridRequest<T> : GridRequest where T : class
{
    public T? Filter { get; set; }

    public override PagedListQuery<T> ToPaginationQuery()
    {
        return new PagedListQuery<T>()
        {
            OrderDirection = OrderDirection,
            Page = Page,
            PageSize = PageSize,
            Filter = Filter
        };
    }
}