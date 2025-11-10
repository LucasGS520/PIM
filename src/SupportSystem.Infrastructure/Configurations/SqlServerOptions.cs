namespace SupportSystem.Infrastructure.Configurations;

// Representa as configurações necessárias para conectar ao banco MS SQL Server.
public class SqlServerOptions
{
    // String de conexão completa utilizada pelo Entity Framework.
    public string ConnectionString { get; set; } = string.Empty;
}
