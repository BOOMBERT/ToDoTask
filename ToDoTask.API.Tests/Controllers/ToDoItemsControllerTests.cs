using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using ToDoTask.Application.ToDoItems.Commands.CreateToDoItem;
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
            ExpiryDateTime = DateTime.Now.AddDays(7).AddHours(1),
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
            Assert.Equal(command.ExpiryDateTime.ToUniversalTime(), toDoItem.ExpiryDateTime);
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
            ExpiryDateTime = DateTime.Now.AddDays(-1),
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
}