using Microsoft.EntityFrameworkCore;
using ToDoTask.API.Middlewares;
using ToDoTask.Infrastructure.Persistence;

namespace ToDoTask.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (dbContext.Database.GetPendingMigrations().Any())
                await dbContext.Database.MigrateAsync();
        }
    }

    public static void ApplyMiddlewares(this WebApplication app) 
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
