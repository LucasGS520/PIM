using Microsoft.EntityFrameworkCore;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;
using SupportSystem.Domain.Entities;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.Services;

/// <summary>
/// Implementa cálculo de indicadores gerenciais em tempo real.
/// </summary>
public class ReportingService : IReportingService
{
    private readonly IRepository<Ticket> _tickets;
    private readonly IRepository<Feedback> _feedbacks;

    /// <summary>
    /// Inicializa o serviço com repositórios necessários.
    /// </summary>
    public ReportingService(IRepository<Ticket> tickets, IRepository<Feedback> feedbacks)
    {
        _tickets = tickets;
        _feedbacks = feedbacks;
    }

    /// <inheritdoc />
    public async Task<DashboardMetricsDto> GetDashboardMetricsAsync(DateTime referenceDate, CancellationToken cancellationToken)
    {
        var startOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var ticketsQuery = _tickets.Query().AsNoTracking();
        var feedbackQuery = _feedbacks.Query().AsNoTracking();

        var resolvedTickets = await ticketsQuery
            .Where(t => t.Status == TicketStatus.Resolvido && t.ClosedAt >= startOfMonth && t.ClosedAt < endOfMonth)
            .ToListAsync(cancellationToken);

        var timeToResolve = resolvedTickets
            .Where(t => t.ClosedAt.HasValue)
            .Select(t => (t.ClosedAt!.Value - t.CreatedAt).TotalHours)
            .DefaultIfEmpty(0)
            .Average();

        var openTickets = await ticketsQuery.CountAsync(t => t.Status is TicketStatus.Aberto or TicketStatus.EmAndamento or TicketStatus.AguardandoUsuario, cancellationToken);

        var ticketsByCategory = await ticketsQuery
            .GroupBy(t => t.Category)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key ?? "Indefinido", x => x.Count, cancellationToken);

        var ticketsByTechnician = await ticketsQuery
            .Where(t => t.AssigneeId != null)
            .GroupBy(t => t.AssigneeId!)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count, cancellationToken);

        var feedbacks = await feedbackQuery.ToListAsync(cancellationToken);
        var satisfaction = feedbacks.Count == 0 ? 0 : feedbacks.Average(f => f.Score);

        var reopenedRate = await ticketsQuery.CountAsync(t => t.Status == TicketStatus.Reaberto, cancellationToken);
        var totalTickets = await ticketsQuery.CountAsync(cancellationToken);
        var reopenPercentage = totalTickets == 0 ? 0 : reopenedRate / (double)totalTickets;

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
