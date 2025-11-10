using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.Services;

// Implementa o cálculo de indicadores gerenciais em tempo real.
// Serviço responsável por agregar métricas a partir dos repositórios de Tickets e Feedbacks.
public class ReportingService : IReportingService
{
    // Repositório de tickets (fonte principal das métricas de atendimento).
    private readonly IRepository<Ticket> _tickets;

    // Repositório de feedbacks (fonte da métrica de satisfação).
    private readonly IRepository<Feedback> _feedbacks;

    // Inicializa o serviço com os repositórios necessários.
    public ReportingService(IRepository<Ticket> tickets, IRepository<Feedback> feedbacks)
    {
        _tickets = tickets;
        _feedbacks = feedbacks;
    }

    // Calcula e retorna as métricas do dashboard para o mês de referência fornecido.
    public async Task<DashboardMetricsDto> GetDashboardMetricsAsync(DateTime referenceDate, CancellationToken cancellationToken)
    {
        // Determina o intervalo do mês (início inclusive, fim exclusivo).
        var startOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        // Base das queries com AsNoTracking para melhorar performance de leitura.
        var ticketsQuery = _tickets.Query().AsNoTracking();
        var feedbackQuery = _feedbacks.Query().AsNoTracking();

        // Tickets resolvidos no mês de referência.
        var resolvedTickets = await ticketsQuery
            .Where(t => t.Status == TicketStatus.Resolvido && t.ClosedAt >= startOfMonth && t.ClosedAt < endOfMonth)
            .ToListAsync(cancellationToken);

        // Calcula tempo médio em horas entre criação e fechamento para tickets resolvidos.
        var timeToResolve = resolvedTickets
            .Where(t => t.ClosedAt.HasValue)
            .Select(t => (t.ClosedAt!.Value - t.CreatedAt).TotalHours)
            .DefaultIfEmpty(0)
            .Average();

        // Contagem de tickets que estão considerados abertos (vários status).
        var openTickets = await ticketsQuery.CountAsync(
            t => t.Status is TicketStatus.Aberto or TicketStatus.EmAndamento or TicketStatus.AguardandoUsuario,
            cancellationToken
        );

        // Agrupa tickets por categoria e converte para dicionário.
        // Usa "Indefinido" para categorias nulas.
        var ticketsByCategory = await ticketsQuery
            .GroupBy(t => t.Category)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key ?? "Indefinido", x => x.Count, cancellationToken);

        // Agrupa tickets por técnico responsável e converte para dicionário.
        // Exclui tickets sem técnico atribuído.
        var ticketsByTechnician = await ticketsQuery
            .Where(t => t.AssigneeId != null)
            .GroupBy(t => t.AssigneeId!)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count, cancellationToken);

        // Carrega todos os feedbacks para cálculo da satisfação média.
        var feedbacks = await feedbackQuery.ToListAsync(cancellationToken);
        var satisfaction = feedbacks.Count == 0 ? 0 : feedbacks.Average(f => f.Score);

        // Calcula percentual de tickets reabertos (número reabertos / total).
        var reopenedRate = await ticketsQuery.CountAsync(t => t.Status == TicketStatus.Reaberto, cancellationToken);
        var totalTickets = await ticketsQuery.CountAsync(cancellationToken);
        var reopenPercentage = totalTickets == 0 ? 0 : reopenedRate / (double)totalTickets;

        // Monta e retorna o DTO com todas as métricas calculadas.
        return new DashboardMetricsDto(
            timeToResolve,
            openTickets,
            resolvedTickets.Count,
            ticketsByCategory,
            ticketsByTechnician,
            satisfaction,
            reopenPercentage
        );
    }
}
