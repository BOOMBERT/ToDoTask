using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoTask.Infrastructure.Persistence;

namespace ToDoTask.API.Tests;

internal class TestDatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public TestDatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ConfigureDatabaseAsync()
    {
        using (var scope = _serviceProvider.CreateScope()) 
        { 
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            await ClearDatabaseAsync(db);
        }
    }

    private async Task ClearDatabaseAsync(AppDbContext dbContext)
    {
        foreach (var entityType in dbContext.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            var sql = $"DELETE FROM {tableName}";

            await dbContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
