using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ToDoTask.Application.ToDoItems.Commands.CreateToDoItem;
using ToDoTask.Application.ToDoItems.Commands.UpdateToDoItem;
using ToDoTask.Application.ToDoItems.Dtos;
using ToDoTask.Domain.Entities;
using ToDoTask.Infrastructure.Persistence;
using Xunit;

namespace ToDoTask.API.Tests.Controllers;

public class ToDoItemsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestDatabaseInitializer _dbInitializer;

    private const string BASE_ROUTE_PATH = "/api/todoitems";

    public ToDoItemsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
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

        var response = await _client.PostAsync(BASE_ROUTE_PATH, content);

        // Assert

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var toDoItem = await db.ToDoItems.SingleOrDefaultAsync();

            Assert.NotNull(toDoItem);
            Assert.Equal(command.Title, toDoItem.Title);
            Assert.Equal(command.Description, toDoItem.Description);
            Assert.Equal(command.ExpiryDateTimeUtc, toDoItem.ExpiryDateTimeUtc);
            Assert.Equal(command.CompletionPercentage, toDoItem.CompletionPercentage);

            Assert.Equal($"http://localhost/api/todoitems/{toDoItem.Id}", response.Headers.Location?.ToString());
        }
    }

    [Fact]
    public async Task CreateToDoItem_WhenInvalidRequest_ShouldReturn400BadRequest()
    {
        // Arrange

        var invalidCommand = new CreateToDoItemCommand
        {
            Title = "",
            Description = new string('A', 513),
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(-1),
            CompletionPercentage = 123.456789m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(invalidCommand),
            Encoding.UTF8,
            "application/json"
        );

        // Act

        var response = await _client.PostAsync(BASE_ROUTE_PATH, content);

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

        var toDoItem = new ToDoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(7).AddHours(1),
            CompletionPercentage = 12.34m
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.AddAsync(toDoItem);
            await db.SaveChangesAsync();
        }

        // Act

        var response = await _client.GetAsync($"{BASE_ROUTE_PATH}/{toDoItem.Id}");
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

        var response = await _client.GetAsync($"{BASE_ROUTE_PATH}/{Guid.Empty}");

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

        var toDoItem = new ToDoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(7).AddHours(1),
            CompletionPercentage = 12.34m
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
         
            await db.AddAsync(toDoItem);
            await db.SaveChangesAsync();
        }

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

        var response = await _client.PutAsync($"{BASE_ROUTE_PATH}/{toDoItem.Id}", content);

        // Assert

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var updatedToDoItem = await db.ToDoItems.SingleOrDefaultAsync();

            Assert.NotNull(updatedToDoItem);
            Assert.Equal(toDoItem.Id, updatedToDoItem.Id);
            Assert.Equal(command.Title, updatedToDoItem.Title);
            Assert.Equal(command.Description, updatedToDoItem.Description);
            Assert.Equal(command.ExpiryDateTimeUtc, updatedToDoItem.ExpiryDateTimeUtc);
            Assert.Equal(command.CompletionPercentage, updatedToDoItem.CompletionPercentage);
        }
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
            ExpiryDateTimeUtc = DateTime.UtcNow.AddDays(-1),
            CompletionPercentage = 123.456789m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(invalidCommand),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        
        var response = await _client.PutAsync($"{BASE_ROUTE_PATH}/{invalidCommand.Id}", content);
        
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
        
        var response = await _client.PutAsync($"{BASE_ROUTE_PATH}/{command.Id}", content);
        
        // Assert
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}