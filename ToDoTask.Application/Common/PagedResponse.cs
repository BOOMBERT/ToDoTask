namespace ToDoTask.Application.Common;

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; }
    public PaginationInfo Pagination { get; set; }

    public PagedResponse(IEnumerable<T> Data, PaginationInfo Pagination)
    {
        this.Data = Data;
        this.Pagination = Pagination;
    }
}
