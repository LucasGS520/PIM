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

// Expõe métodos de extensão para registrar os serviços de infraestrutura.
public static class DependencyInjection
{
    // Registra dependências da camada de infraestrutura no container da aplicação.
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configura opções fortemente tipadas para a seção "Database" do appsettings.
        services.Configure<SqlServerOptions>(configuration.GetSection("Database"));

        // Recupera e valida a string de conexão do banco de dados.
        var connectionString = configuration.GetSection("Database:ConnectionString").Value
            ?? throw new InvalidOperationException("A string de conexão do banco não foi configurada.");

        // Registra o DbContext do Entity Framework Core utilizando SQL Server.
        services.AddDbContext<SupportSystemDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositório genérico para entidades e implementação da unidade de trabalho.
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Cliente do assistente (AI). Aqui está configurado para usar uma implementação
        services.AddScoped<IAClienteAssistente, ClienteAssistenteBaseadoEmRegras>();

        // Registramos serviços da camada de aplicação para serem consumidos pelos controladores.
        // Cada serviço encapsula regras de negócio e orquestra operações entre repositórios e infraestrutura.
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<INotificationService, NotificationService>();

        // Expõe INotificationDispatcher resolvendo a implementação concreta NotificationService.
        // Casting explícito porque NotificationService implementa ambas as interfaces.
        services.AddScoped<INotificationDispatcher>(sp => (NotificationService)sp.GetRequiredService<INotificationService>());

        // Serviço para relatórios, agregando dados e prepare output para consumidores.
        services.AddScoped<IReportingService, ReportingService>();

        return services;
    }
}
