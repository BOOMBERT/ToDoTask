using MediatR;
using ToDoTask.Application.Common;
using ToDoTask.Application.Interfaces;
using ToDoTask.Application.ToDoItems.Dtos;
using ToDoTask.Application.Utils;
using ToDoTask.Domain.Constants;
using ToDoTask.Domain.Repositories;

namespace ToDoTask.Application.ToDoItems.Queries.GetAllToDoItems;

public class GetAllToDoItemsQueryHandler : IRequestHandler<GetAllToDoItemsQuery, PagedResponse<ToDoItemDto>>
{
    private readonly IToDoItemsRepository _toDoItemsRepository;
    private readonly IClock _clock;

    public GetAllToDoItemsQueryHandler(IToDoItemsRepository toDoItemsRepository, IClock clock)
    {
        _toDoItemsRepository = toDoItemsRepository;
        _clock = clock;
    }

    public async Task<PagedResponse<ToDoItemDto>> Handle(GetAllToDoItemsQuery request, CancellationToken cancellationToken)
    {
        DateTime? filterExpiryDateTimeUtcStart = null, filterExpiryDateTimeUtcEnd = null;

        if (request.TimeZoneId != null && request.DateTimeRangeFilter != null)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZoneId);
            
            (filterExpiryDateTimeUtcStart, filterExpiryDateTimeUtcEnd) = DateTimeUtil.GetUtcDateRange(
                (DateTimeRange)request.DateTimeRangeFilter, timeZoneInfo, _clock);
        }

        var (toDoItems, totalCount) = await _toDoItemsRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            filterExpiryDateTimeUtcStart,
            filterExpiryDateTimeUtcEnd);

        var toDoItemsDtos = toDoItems.Select(
            toDoItem => new ToDoItemDto(
                toDoItem.Id, 
                toDoItem.Title, 
                toDoItem.Description, 
                toDoItem.ExpiryDateTimeUtc, 
                toDoItem.CompletionPercentage)
            );

        var paginationInfo = new PaginationInfo(totalCount, request.PageSize, request.PageNumber);

        return new PagedResponse<ToDoItemDto>(toDoItemsDtos, paginationInfo);
    }
}
