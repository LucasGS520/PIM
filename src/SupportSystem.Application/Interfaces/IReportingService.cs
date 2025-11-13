using System;
using System.Threading;
using System.Threading.Tasks;
using SupportSystem.Application.DTOs;

namespace SupportSystem.Application.Interfaces;

// Define operações responsáveis pelo cálculo de indicadores gerenciais usados em dashboards e relatórios.
// Implementações típicas agregam dados de múltiplas fontes e retornam DTOs prontos para apresentação.
public interface IReportingService
{
    // Calcula métricas consolidadas do período atual com base na data de referência informada.
    // Data que serve como referência para o cálculo (por exemplo: início do mês, dia atual ou período desejado).
    // Use essa data para determinar o intervalo de agregação das métricas.
    Task<MetricasDashboardDto> GetDashboardMetricsAsync(DateTime referenceDate, CancellationToken cancellationToken);
}
