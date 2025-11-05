using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers
{
    // Controlador responsável pela exposição de relatórios gerenciais.
    // Fornece endpoints públicos para recuperação de métricas e informações do painel de gestão.
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        // Serviço de relatório injetado via DI.
        // Responsável pela lógica de obtenção das métricas e agregações.
        private readonly IReportingService _reportingService;

        // Construtor com injeção do serviço de relatórios.
        public ReportsController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

 
        // Recupera as métricas consolidadas para o painel gerencial.
        // Retorna um objeto com as principais estatísticas usadas no dashboard.
        [HttpGet("dashboard")]
        public async Task<ActionResult> GetDashboardMetricsAsync([FromQuery] DateTime? referenceDate = null, CancellationToken cancellationToken = default)
        {
            // Determina a data de referência: recebe a informada ou usa o UTC atual como fallback.
            var date = referenceDate ?? DateTime.UtcNow;

            // Chama o serviço de relatório para recuperar as métricas consolidadas.
            var metrics = await _reportingService.GetDashboardMetricsAsync(date, cancellationToken);

            // Retorna 200 OK com o payload de métricas.
            return Ok(metrics);
        }
    }
}
