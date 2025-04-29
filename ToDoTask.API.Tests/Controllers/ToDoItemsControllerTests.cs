using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ToDoTask.Application.Common;
using ToDoTask.Application.ToDoItems.Commands.CreateToDoItem;
using ToDoTask.Application.ToDoItems.Commands.SetToDoItemCompletionPercentage;
using ToDoTask.Application.ToDoItems.Commands.UpdateToDoItem;
using ToDoTask.Application.ToDoItems.Dtos;
using ToDoTask.Domain.Constants;
using ToDoTask.Domain.Entities;
using ToDoTask.Infrastructure.Persistence;
using Xunit;

namespace ToDoTask.API.Tests.Controllers;

public class ToDoItemsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestDatabaseInitializer _dbInitializer;

    private const string BaseRoutePath = "/api/todoitems";
    private const string PacificHonoluluTimeZoneId = "Pacific/Honolulu"; // Constant difference of -10 hours from UTC
    private readonly DateTime fixedUtcNow = new DateTime(2025, 4, 1, 5, 30, 15, DateTimeKind.Utc); // 01.04.2025 05:30:15 UTC | 31.03.2025 19:30:15 HST

    public ToDoItemsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        factory.FixedUtcNow = fixedUtcNow;
        _factory = factory;
        _client = factory.CreateClient();
        _dbInitializer = new TestDatabaseInitializer(factory.Services);
    }

    #region Test_CreateToDoItem

    [Fact]
    public async Task CreateToDoItem_WhenValidRequest_ShouldCreateToDoItemAndReturn201Created()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var command = new CreateToDoItemCommand
        {
            Title = "Test Title",
            Description = "Test Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(7).AddHours(1),
            CompletionPercentage = 12.34m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json"
        );

        // Act

        var response = await _client.PostAsync(BaseRoutePath, content);

        // Assert

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdToDoItem = await GetSingleToDoItemAsync();

        Assert.NotNull(createdToDoItem);
        Assert.Equal(command.Title, createdToDoItem.Title);
        Assert.Equal(command.Description, createdToDoItem.Description);
        Assert.Equal(command.ExpiryDateTimeUtc, createdToDoItem.ExpiryDateTimeUtc);
        Assert.Equal(command.CompletionPercentage, createdToDoItem.CompletionPercentage);

        Assert.Equal($"http://localhost/api/todoitems/{createdToDoItem.Id}", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task CreateToDoItem_WhenInvalidRequest_ShouldReturn400BadRequest()
    {
        // Arrange

        var invalidCommand = new CreateToDoItemCommand
        {
            Title = "",
            Description = new string('A', 513),
            ExpiryDateTimeUtc = fixedUtcNow.AddDays(-1),
            CompletionPercentage = 123.456789m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(invalidCommand),
            Encoding.UTF8,
            "application/json"
        );

        // Act

        var response = await _client.PostAsync(BaseRoutePath, content);

        // Assert

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Test_GetToDoItemById

    [Fact]
    public async Task GetToDoItemById_WhenValidRequest_ShouldReturnSpecifiedToDoItemDtoAnd200Ok()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var toDoItem = await CreateAndSaveToDoItemAsync();

        // Act

        var response = await _client.GetAsync($"{BaseRoutePath}/{toDoItem.Id}");
        var toDoItemDtoFromResponse = await response.Content.ReadFromJsonAsync<ToDoItemDto>();

        // Assert

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(toDoItemDtoFromResponse);
        Assert.Equal(toDoItem.Id, toDoItemDtoFromResponse.Id);
        Assert.Equal(toDoItem.Title, toDoItemDtoFromResponse.Title);
        Assert.Equal(toDoItem.Description, toDoItemDtoFromResponse.Description);
        Assert.Equal(toDoItem.ExpiryDateTimeUtc, toDoItemDtoFromResponse.ExpiryDateTimeUtc);
        Assert.Equal(toDoItem.CompletionPercentage, toDoItemDtoFromResponse.CompletionPercentage);
    }

    [Fact]
    public async Task GetToDoItemById_WhenNonExistingToDoItem_ShouldReturn404NotFound() 
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        // Act

        var response = await _client.GetAsync($"{BaseRoutePath}/{Guid.Empty}");

        // Assert

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Test_UpdateToDoItem

    [Fact]
    public async Task UpdateToDoItem_WhenValidRequest_ShouldUpdateSpecifiedToDoItemAndReturn204NoContent()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var toDoItem = await CreateAndSaveToDoItemAsync();

        var command = new UpdateToDoItemCommand(toDoItem.Id)
        {
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(14).AddHours(2),
            CompletionPercentage = 56.78m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json"
        );

        // Act

        var response = await _client.PutAsync($"{BaseRoutePath}/{toDoItem.Id}", content);

        // Assert

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedToDoItem = await GetSingleToDoItemAsync(toDoItem.Id);

        Assert.NotNull(updatedToDoItem);
        Assert.Equal(toDoItem.Id, updatedToDoItem.Id);
        Assert.Equal(command.Title, updatedToDoItem.Title);
        Assert.Equal(command.Description, updatedToDoItem.Description);
        Assert.Equal(command.ExpiryDateTimeUtc, updatedToDoItem.ExpiryDateTimeUtc);
        Assert.Equal(command.CompletionPercentage, updatedToDoItem.CompletionPercentage);
    }

    [Fact]
    public async Task UpdateToDoItem_WhenInvalidRequest_ShouldReturn400BadRequest()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var invalidCommand = new UpdateToDoItemCommand(Guid.NewGuid())
        {
            Title = "",
            Description = new string('A', 513),
            ExpiryDateTimeUtc = fixedUtcNow.AddDays(-1),
            CompletionPercentage = 123.456789m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(invalidCommand),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        
        var response = await _client.PutAsync($"{BaseRoutePath}/{invalidCommand.Id}", content);
        
        // Assert

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateToDoItem_WhenNonExistingToDoItem_ShouldReturn404NotFound()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();
        
        var command = new UpdateToDoItemCommand(Guid.NewGuid())
        {
            Title = "Updated Title",
            Description = "Updated Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(14).AddHours(2),
            CompletionPercentage = 56.78m
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json"
        );
        
        // Act
        
        var response = await _client.PutAsync($"{BaseRoutePath}/{command.Id}", content);
        
        // Assert
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Test_DeleteToDoItem

    [Fact]
    public async Task DeleteToDoItem_WhenValidRequest_ShouldDeleteSpecifiedToDoItemAndReturn204NoContent()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var toDoItem = await CreateAndSaveToDoItemAsync();

        // Act
        
        var response = await _client.DeleteAsync($"{BaseRoutePath}/{toDoItem.Id}");
        
        // Assert
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        var deletedToDoItem = await GetSingleToDoItemAsync(toDoItem.Id);

        Assert.Null(deletedToDoItem);
    }

    [Fact]
    public async Task DeleteToDoItem_WhenNonExistingToDoItem_ShouldReturn404NotFound()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        // Act

        var response = await _client.DeleteAsync($"{BaseRoutePath}/{Guid.Empty}");

        // Assert

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Test_SetToDoItemCompletionPercentage

    [Fact]
    public async Task SetToDoItemCompletionPercentage_WhenValidRequest_ShouldSetToDoItemCompletionPercentageAndReturn204NoContent()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var toDoItem = await CreateAndSaveToDoItemAsync();

        var command = new SetToDoItemCompletionPercentageCommand
        {
            CompletionPercentage = 75.00m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json"
        );

        // Act

        var response = await _client.PatchAsync($"{BaseRoutePath}/{toDoItem.Id}/completion-percentage", content);

        // Assert

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedToDoItem = await GetSingleToDoItemAsync(toDoItem.Id);

        Assert.NotNull(updatedToDoItem);
        Assert.Equal(command.CompletionPercentage, updatedToDoItem.CompletionPercentage);
        Assert.Equal(toDoItem.Id, updatedToDoItem.Id);
        Assert.Equal(toDoItem.Title, updatedToDoItem.Title);
        Assert.Equal(toDoItem.Description, updatedToDoItem.Description);
        Assert.Equal(toDoItem.ExpiryDateTimeUtc, updatedToDoItem.ExpiryDateTimeUtc);
    }

    [Fact]
    public async Task SetToDoItemCompletionPercentage_WhenInvalidRequest_ShouldReturn400BadRequest()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var invalidCommand = new SetToDoItemCompletionPercentageCommand
        {
            CompletionPercentage = 123.456789m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(invalidCommand),
            Encoding.UTF8,
            "application/json"
        );

        // Act

        var response = await _client.PatchAsync($"{BaseRoutePath}/{Guid.Empty}/completion-percentage", content);

        // Assert

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SetToDoItemCompletionPercentage_WhenNonExistingToDoItem_ShouldReturn404NotFound()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var command = new SetToDoItemCompletionPercentageCommand
        {
            CompletionPercentage = 75.00m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json"
        );

        // Act

        var response = await _client.PatchAsync($"{BaseRoutePath}/{Guid.Empty}/completion-percentage", content);

        // Assert

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Test_MarkToDoItemAsDone

    [Fact]
    public async Task MarkToDoItemAsDone_WhenValidRequest_ShouldSetToDoItemCompletionPercentageTo100AndReturn204NoContent()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var toDoItem = await CreateAndSaveToDoItemAsync();

        // Act

        var response = await _client.PatchAsync($"{BaseRoutePath}/{toDoItem.Id}/mark-as-done", null);

        // Assert

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedToDoItem = await GetSingleToDoItemAsync(toDoItem.Id);

        Assert.NotNull(updatedToDoItem);
        Assert.Equal(100m, updatedToDoItem.CompletionPercentage);
        Assert.Equal(toDoItem.Id, updatedToDoItem.Id);
        Assert.Equal(toDoItem.Title, updatedToDoItem.Title);
        Assert.Equal(toDoItem.Description, updatedToDoItem.Description);
        Assert.Equal(toDoItem.ExpiryDateTimeUtc, updatedToDoItem.ExpiryDateTimeUtc);
    }

    [Fact]
    public async Task MarkToDoItemAsDone_WhenNonExistingToDoItem_ShouldReturn404NotFound()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        // Act

        var response = await _client.PatchAsync($"{BaseRoutePath}/{Guid.Empty}/mark-as-done", null);

        // Assert

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Test_GetAllToDoItems

    [Fact]
    public async Task GetAllToDoItems_WhenNoFiltersAndNoSort_ShouldReturnAllToDoItemsWithPaginationAnd200Ok()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var totalToDoItems = 5 + 4 + 3 + 2;
        var pageSize = 20;
        var toDoItemsDtos = await CreateAndSaveToDoItemsForSpecifiedDateRangeAsync(5, 4, 3, 2);

        var queryParameters = new Dictionary<string, string?>
        {
            ["PageNumber"] = "1",
            ["PageSize"] = pageSize.ToString(),
        };

        var url = QueryHelpers.AddQueryString(BaseRoutePath, queryParameters);

        // Act

        var response = await _client.GetAsync(url);
        var toDoItemsDtosAndPaginationInfoFromResponse = await response.Content.ReadFromJsonAsync<PagedResponse<ToDoItemDto>>();
        var (toDoItemsDtosFromResponse, paginationInfoFromResponse) = 
            (toDoItemsDtosAndPaginationInfoFromResponse?.Data, toDoItemsDtosAndPaginationInfoFromResponse?.Pagination);

        // Assert

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(toDoItemsDtosFromResponse);
        Assert.NotNull(paginationInfoFromResponse);

        Assert.Equal(totalToDoItems, toDoItemsDtosFromResponse.Count());
        
        Assert.Equal(1, paginationInfoFromResponse.CurrentPage);
        Assert.Equal(pageSize, paginationInfoFromResponse.PageSize);
        Assert.Equal(1, paginationInfoFromResponse.TotalPageCount);
        Assert.Equal(totalToDoItems, paginationInfoFromResponse.TotalItemCount);

        Assert.Equal(
            toDoItemsDtos.Select(t => t.Id).OrderBy(id => id), 
            toDoItemsDtosFromResponse.Select(t => t.Id).OrderBy(id => id)
        );
    }

    [Theory]
    [InlineData(DateTimeRange.Today)]
    [InlineData(DateTimeRange.Tomorrow)]
    [InlineData(DateTimeRange.ThisWeek)]
    public async Task GetAllToDoItems_WhenFilteredByDateTimeRangeAndValidTimeZone_ShouldReturnExpectedItemsWithPaginationAnd200Ok(DateTimeRange dateTimeRange)
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var pageSize = 20;
        var toDoItemsDtos = await CreateAndSaveToDoItemsForSpecifiedDateRangeAsync(5, 4, 3, 2);

        var queryParameters = new Dictionary<string, string?>
        {
            ["PageNumber"] = "1",
            ["PageSize"] = pageSize.ToString(),
            ["DateTimeRangeFilter"] = dateTimeRange.ToString(),
            ["TimeZoneId"] = PacificHonoluluTimeZoneId
        };

        var url = QueryHelpers.AddQueryString(BaseRoutePath, queryParameters);

        // Act

        var response = await _client.GetAsync(url);
        var toDoItemsDtosAndPaginationInfoFromResponse = await response.Content.ReadFromJsonAsync<PagedResponse<ToDoItemDto>>();
        var (toDoItemsDtosFromResponse, paginationInfoFromResponse) =
            (toDoItemsDtosAndPaginationInfoFromResponse?.Data, toDoItemsDtosAndPaginationInfoFromResponse?.Pagination);

        // Assert

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(toDoItemsDtosFromResponse);
        Assert.NotNull(paginationInfoFromResponse);

        var allToDoItemsIdsInDb = toDoItemsDtos.Select(t => t.Id);

        if (dateTimeRange == DateTimeRange.Today)
        {
            Assert.Equal(5, toDoItemsDtosFromResponse.Count());
            Assert.Equal(5, paginationInfoFromResponse.TotalItemCount);

            Assert.True(toDoItemsDtosFromResponse.All(t => t.Title.StartsWith("Today") && allToDoItemsIdsInDb.Contains(t.Id)));
        } 
        else if (dateTimeRange == DateTimeRange.Tomorrow)
        {
            Assert.Equal(4, toDoItemsDtosFromResponse.Count());
            Assert.Equal(4, paginationInfoFromResponse.TotalItemCount);

            Assert.True(toDoItemsDtosFromResponse.All(t => t.Title.StartsWith("Tomorrow") && allToDoItemsIdsInDb.Contains(t.Id)));
        }
        else if (dateTimeRange == DateTimeRange.ThisWeek)
        {
            Assert.Equal(12, toDoItemsDtosFromResponse.Count());
            Assert.Equal(12, paginationInfoFromResponse.TotalItemCount);

            Assert.True(toDoItemsDtosFromResponse.All(t => 
                (t.Title.StartsWith("Today") || t.Title.StartsWith("Tomorrow") || t.Title.StartsWith("This Week")) && 
                allToDoItemsIdsInDb.Contains(t.Id))
            );
        }

        Assert.Equal(1, paginationInfoFromResponse.CurrentPage);
        Assert.Equal(pageSize, paginationInfoFromResponse.PageSize);
        Assert.Equal(1, paginationInfoFromResponse.TotalPageCount);
    }

    [Fact]
    public async Task GetAllToDoItems_WhenInvalidQuery_ShouldReturn400BadRequest()
    {
        // Arrange

        var queryParameters = new Dictionary<string, string?>
        {
            ["PageNumber"] = "0",
            ["PageSize"] = "0",
            ["SearchPhrase"] = "     ",
            ["SortBy"] = "Invalid SortBy",
            ["TimeZoneId"] = "Invalid/TimeZoneId",
        };

        var url = QueryHelpers.AddQueryString(BaseRoutePath, queryParameters);

        // Act

        var response = await _client.GetAsync(url);

        // Assert

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAllToDoItems_WhenSortDescByExpiryDateTimeUtcAndNoFilters_ShouldReturnSortedDescAllToDoItemsWithPaginationAnd200Ok()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var pageSize = 20;
        var toDoItemsDtos = await CreateAndSaveToDoItemsForSpecifiedDateRangeAsync(5, 4, 3, 2);

        var queryParameters = new Dictionary<string, string?>
        {
            ["PageNumber"] = "1",
            ["PageSize"] = pageSize.ToString(),
            ["SortBy"] = "ExpiryDateTimeUtc",
            ["SortDirection"] = SortDirection.Descending.ToString()
        };

        var url = QueryHelpers.AddQueryString(BaseRoutePath, queryParameters);

        // Act

        var response = await _client.GetAsync(url);
        var toDoItemsDtosAndPaginationInfoFromResponse = await response.Content.ReadFromJsonAsync<PagedResponse<ToDoItemDto>>();
        var (toDoItemsDtosFromResponse, paginationInfoFromResponse) =
            (toDoItemsDtosAndPaginationInfoFromResponse?.Data, toDoItemsDtosAndPaginationInfoFromResponse?.Pagination);

        // Assert

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(toDoItemsDtosFromResponse);
        Assert.NotNull(paginationInfoFromResponse);

        Assert.Equal(14, toDoItemsDtosFromResponse.Count());

        Assert.Equal(14, paginationInfoFromResponse.TotalItemCount);
        Assert.Equal(1, paginationInfoFromResponse.CurrentPage);
        Assert.Equal(pageSize, paginationInfoFromResponse.PageSize);
        Assert.Equal(1, paginationInfoFromResponse.TotalPageCount);

        Assert.Equal(
            toDoItemsDtos.Select(t => t.Id).OrderBy(id => id),
            toDoItemsDtosFromResponse.Select(t => t.Id).OrderBy(id => id)
        );

        Assert.Equal(
            toDoItemsDtos.Select(t => t.ExpiryDateTimeUtc).OrderByDescending(dt => dt),
            toDoItemsDtosFromResponse.Select(t => t.ExpiryDateTimeUtc)
        );
    }

    [Fact]
    public async Task GetAllToDoItems_WhenSearchPhraseMatchesAndMultiplePages_ShouldReturnMatchingItemsWithPaginationAnd200Ok()
    {
        // Arrange

        await _dbInitializer.ConfigureDatabaseAsync();

        var pageSize = 5;
        var toDoItemsDtos = await CreateAndSaveToDoItemsForSpecifiedDateRangeAsync(5, 4, 7, 2);

        var queryParameters = new Dictionary<string, string?>
        {
            ["PageNumber"] = "2",
            ["PageSize"] = pageSize.ToString(),
            ["SearchPhrase"] = "This Week"
        };

        var url = QueryHelpers.AddQueryString(BaseRoutePath, queryParameters);

        // Act

        var response = await _client.GetAsync(url);
        var toDoItemsDtosAndPaginationInfoFromResponse = await response.Content.ReadFromJsonAsync<PagedResponse<ToDoItemDto>>();
        var (toDoItemsDtosFromResponse, paginationInfoFromResponse) =
            (toDoItemsDtosAndPaginationInfoFromResponse?.Data, toDoItemsDtosAndPaginationInfoFromResponse?.Pagination);

        // Assert

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(toDoItemsDtosFromResponse);
        Assert.NotNull(paginationInfoFromResponse);

        Assert.Equal(2, toDoItemsDtosFromResponse.Count());

        Assert.Equal(7, paginationInfoFromResponse.TotalItemCount);
        Assert.Equal(2, paginationInfoFromResponse.CurrentPage);
        Assert.Equal(pageSize, paginationInfoFromResponse.PageSize);
        Assert.Equal(2, paginationInfoFromResponse.TotalPageCount);

        var allToDoItemsIdsInDb = toDoItemsDtos.Select(t => t.Id);
        Assert.True(toDoItemsDtosFromResponse.All(t => 
            (t.Title.StartsWith("This Week") || t.Description.StartsWith("This Week")) && 
            allToDoItemsIdsInDb.Contains(t.Id))
        );
    }

    #endregion

    #region Test_Data

    private async Task<ToDoItem> CreateAndSaveToDoItemAsync(
        string title = "Test Title",
        string description = "Test Description",
        decimal completionPercentage = 12.34m,
        DateTime? expiryDateTimeUtc = null)
    {
        var toDoItem = new ToDoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            ExpiryDateTimeUtc = expiryDateTimeUtc ?? DateTime.UtcNow.AddDays(7).AddMinutes(30),
            CompletionPercentage = completionPercentage
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.AddAsync(toDoItem);
            await db.SaveChangesAsync();
        }

        return toDoItem;
    }

    private async Task<ToDoItem?> GetSingleToDoItemAsync(Guid? id = null)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return id.HasValue
                ? await db.ToDoItems.SingleOrDefaultAsync(t => t.Id == id.Value)
                : await db.ToDoItems.SingleOrDefaultAsync();
        }
    }

    private async Task<IEnumerable<ToDoItem>> CreateAndSaveToDoItemsForSpecifiedDateRangeAsync(int today, int tomorrow, int thisWeekAddition, int notThisWeek)
    {
        var random = new Random();

        var toDoItems = new List<ToDoItem>();

        var todayMaxNegativeHoursOffset = -19; // 31.03.2025 19:30:15 HST - 19 hours = 31.03.2025 00:30:15 HST
        var todayMaxPositiveHoursOffset = 4; // 31.03.2025 19:30:15 HST + 4 hours = 31.03.2025 23:30:15 HST
        for (var i = 1; i <= today; i++)
        {
            toDoItems.Add(new ToDoItem
            {
                Id = Guid.NewGuid(),
                Title = $"Today Test Title {i}",
                Description = $"Today Test Description {i}",
                ExpiryDateTimeUtc = GetRandomDateTimeUtcWithLittleAdditionalOffset(random, fixedUtcNow, todayMaxNegativeHoursOffset, todayMaxPositiveHoursOffset),
                CompletionPercentage = GetRandomCompletionPercentage(random)
            });
        }

        var tomorrowMinPositiveHoursOffset = 5; // 31.03.2025 19:30:15 HST + 5 hours = 01.04.2025 00:30:15 HST
        var tomorrowMaxPositiveHoursOffset = 28; // 31.03.2025 19:30:15 HST + 28 hours = 01.04.2025 23:30:15 HST
        for (var i = 1; i <= tomorrow; i++)
        {
            toDoItems.Add(new ToDoItem
            {
                Id = Guid.NewGuid(),
                Title = $"Tomorrow Test Title {i}",
                Description = $"Tomorrow Test Description {i}",
                ExpiryDateTimeUtc = GetRandomDateTimeUtcWithLittleAdditionalOffset(random, fixedUtcNow, tomorrowMinPositiveHoursOffset, tomorrowMaxPositiveHoursOffset),
                CompletionPercentage = GetRandomCompletionPercentage(random)
            });
        }

        var thisWeekMinPositiveHoursOffset = 29; // 31.03.2025 19:30:15 HST + 29 hours = 02.04.2025 00:30:15 HST
        var thisWeekMaxPositiveHoursOffset = 148; // 31.03.2025 19:30:15 HST + 148 hours = 06.04.2025 23:30:15 HST
        for (var i = 1; i <= thisWeekAddition; i++)
        {
            toDoItems.Add(new ToDoItem
            {
                Id = Guid.NewGuid(),
                Title = $"This Week Test Title {i}",
                Description = $"This Week Test Description {i}",
                ExpiryDateTimeUtc = GetRandomDateTimeUtcWithLittleAdditionalOffset(random, fixedUtcNow, thisWeekMinPositiveHoursOffset, thisWeekMaxPositiveHoursOffset),
                CompletionPercentage = GetRandomCompletionPercentage(random)
            });
        }

        for (var i = 1; i <= notThisWeek; i++)
        {
            int daysOffset;
            if (random.Next(2) == 0)
            {
                daysOffset = random.Next(-1000, -7);
            }
            else
            {
                daysOffset = random.Next(8, 1001);
            }

            toDoItems.Add(new ToDoItem
            {
                Id = Guid.NewGuid(),
                Title = $"More Than Week Test Title {i}",
                Description = $"More Than Week Test Description {i}",
                ExpiryDateTimeUtc = fixedUtcNow.AddDays(daysOffset),
                CompletionPercentage = GetRandomCompletionPercentage(random)
            });
        }

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.AddRangeAsync(toDoItems);
            await db.SaveChangesAsync();
        }

        return toDoItems;
    }

    private DateTime GetRandomDateTimeUtcWithLittleAdditionalOffset(Random random, DateTime baseTime, int minOffsetHours, int maxOffsetHours)
    {
        return DateTime.SpecifyKind(baseTime
            .AddHours(random.Next(minOffsetHours, maxOffsetHours + 1))
            .AddMinutes(random.Next(-15, 16))
            .AddSeconds(random.Next(-60, 61)), DateTimeKind.Utc);
    }

    private decimal GetRandomCompletionPercentage(Random random)
    {
        return Math.Round((decimal)(random.NextDouble() * 100), 2);
    }

    #endregion
}