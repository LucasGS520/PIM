namespace SupportSystem.Application.DTOs;

/// <summary>
/// Representa o conjunto principal de indicadores apresentados aos gestores.
/// </summary>
public record DashboardMetricsDto(
    double TempoMedioResolucaoHoras,
    int ChamadosAbertos,
    int ChamadosResolvidosMes,
    IDictionary<string, int> ChamadosPorCategoria,
    IDictionary<string, int> ChamadosPorTecnico,
    double SatisfacaoMedia,
    double TaxaReabertura
);
