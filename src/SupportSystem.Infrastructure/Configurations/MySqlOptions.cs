namespace SupportSystem.Infrastructure.Configurations;

/// <summary>
/// Representa as configurações necessárias para conectar ao banco MySQL.
/// </summary>
public class MySqlOptions
{
    /// <summary>
    /// String de conexão completa utilizada pelo Entity Framework.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
