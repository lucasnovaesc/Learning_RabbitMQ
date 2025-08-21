using Microsoft.EntityFrameworkCore;
using MassTransit;
using RabbitMQ.Bus;
using RabbitMQ.Data;
using RabbitMQ.Data.Repositories;
using RabbitMQ.Services;

namespace RabbitMQ.Extension;

internal static class AppExtensions
{
    public static void AddRabbitMQService(this IServiceCollection services)
    {
        services.AddTransient<IPublishBus, PublishBus>();
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<RelatorioSolicitadoEventConsumer>();
            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(new Uri("amqp://localhost:5672"), host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });
    }

    public static void AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("RabbitMQ")
            );
        });

        // Repositories
        services.AddScoped<IRelatorioRepository, RelatorioRepository>();

        // Services
        services.AddScoped<IRelatorioService, RelatorioService>();
    }
}