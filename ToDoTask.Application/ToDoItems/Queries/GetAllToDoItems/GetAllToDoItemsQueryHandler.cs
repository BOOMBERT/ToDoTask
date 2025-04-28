using MediatR;
using ToDoTask.Application.Common;
using ToDoTask.Application.ToDoItems.Dtos;
using ToDoTask.Domain.Repositories;

namespace ToDoTask.Application.ToDoItems.Queries.GetAllToDoItems;

public class GetAllToDoItemsQueryHandler : IRequestHandler<GetAllToDoItemsQuery, PagedResponse<ToDoItemDto>>
{
    private readonly IToDoItemsRepository _toDoItemsRepository;

    public GetAllToDoItemsQueryHandler(IToDoItemsRepository toDoItemsRepository)
    {
        _toDoItemsRepository = toDoItemsRepository;
    }

    public async Task<PagedResponse<ToDoItemDto>> Handle(GetAllToDoItemsQuery request, CancellationToken cancellationToken)
    {
        var (toDoItems, totalCount) = await _toDoItemsRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDirection);

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
