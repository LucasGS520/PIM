using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;

/// <summary>
/// Controlador dedicado à consulta e envio de notificações.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    /// <summary>
    /// Construtor com injeção do serviço de notificações.
    /// </summary>
    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Envia manualmente uma notificação para um usuário específico.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> SendAsync([FromBody] CreateNotificationDto dto, CancellationToken cancellationToken)
    {
        var notification = await _service.SendAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetByUserAsync), new { userId = notification.UsuarioId }, notification);
    }

    /// <summary>
    /// Lista notificações de um usuário com a opção de incluir as já lidas.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult> GetByUserAsync(Guid userId, [FromQuery] bool includeRead = false, CancellationToken cancellationToken = default)
    {
        var notifications = await _service.GetByUserAsync(userId, includeRead, cancellationToken);
        return Ok(notifications);
    }

    /// <summary>
    /// Marca uma notificação como lida.
    /// </summary>
    [HttpPost("{id:guid}/read")]
    public async Task<ActionResult> MarkAsReadAsync(Guid id, CancellationToken cancellationToken)
    {
        var updated = await _service.MarkAsReadAsync(id, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}
