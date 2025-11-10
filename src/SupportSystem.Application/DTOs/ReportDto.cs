namespace SupportSystem.Application.DTOs;

// Representa o conjunto principal de indicadores apresentados aos gestores.
// DTO (Data Transfer Object) usado para transportar métricas entre camadas da aplicação.
public record DashboardMetricsDto(
    double TempoMedioResolucaoHoras, // Tempo médio de resolução dos chamados no período, em horas.
    int ChamadosAbertos, // Quantidade de chamados atualmente abertos.
    int ChamadosResolvidosMes, // Quantidade de chamados resolvidos no mês.
    IDictionary<string, int> ChamadosPorCategoria, // Dicionário usado para relatórios e gráficos.
    IDictionary<string, int> ChamadosPorTecnico, // Dicionário para distribuição por responsável.
    double SatisfacaoMedia, // Média de satisfação dos usuários no período
    double TaxaReabertura // Percentual de chamados reabertos no período
);
