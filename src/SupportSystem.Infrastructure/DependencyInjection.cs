using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportSystem.Application.AI;
using SupportSystem.Application.Interfaces;
using SupportSystem.Application.Services;
using SupportSystem.Domain.Entities;
using SupportSystem.Infrastructure.AI;
using SupportSystem.Infrastructure.Configurations;
using SupportSystem.Infrastructure.Data;
using SupportSystem.Infrastructure.Repositories;

namespace SupportSystem.Infrastructure;

/// <summary>
/// Expõe métodos de extensão para registrar os serviços de infraestrutura.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra dependências da camada de infraestrutura no container da aplicação.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MySqlOptions>(configuration.GetSection("Database"));

        var connectionString = configuration.GetSection("Database:ConnectionString").Value
            ?? throw new InvalidOperationException("A string de conexão do banco não foi configurada.");

        services.AddDbContext<SupportSystemDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAssistantClient, RuleBasedAssistantClient>();

        // Registramos serviços de aplicação para facilitar o consumo pelos controladores.
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationDispatcher>(sp => (NotificationService)sp.GetRequiredService<INotificationService>());
        services.AddScoped<IReportingService, ReportingService>();

        return services;
    }
}
