using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces;

/// <summary>
/// Define operações responsáveis pelo cálculo de indicadores gerenciais.
/// </summary>
public interface IReportingService
{
    /// <summary>
    /// Calcula métricas consolidadas do período atual.
    /// </summary>
    Task<DashboardMetricsDto> GetDashboardMetricsAsync(DateTime referenceDate, CancellationToken cancellationToken);
}
