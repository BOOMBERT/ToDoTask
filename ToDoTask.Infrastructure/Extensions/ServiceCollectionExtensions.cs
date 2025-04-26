using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoTask.Domain.Repositories;
using ToDoTask.Infrastructure.Persistence;
using ToDoTask.Infrastructure.Repositories;

namespace ToDoTask.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ToDoTaskDatabase");
        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(connectionString));

        services.AddScoped<IGenericRepository, GenericRepository>();
        services.AddScoped<IToDoItemsRepository, ToDoItemsRepository>();
    }
}
