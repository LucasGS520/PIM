using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;

/// <summary>
/// Controlador responsável pela exposição de relatórios gerenciais.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    /// <summary>
    /// Construtor com injeção do serviço de relatórios.
    /// </summary>
    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    /// <summary>
    /// Recupera as métricas consolidadas para o painel gerencial.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboardMetricsAsync([FromQuery] DateTime? referenceDate = null, CancellationToken cancellationToken = default)
    {
        var date = referenceDate ?? DateTime.UtcNow;
        var metrics = await _reportingService.GetDashboardMetricsAsync(date, cancellationToken);
        return Ok(metrics);
    }
}
