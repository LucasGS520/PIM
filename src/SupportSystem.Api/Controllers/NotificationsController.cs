using Microsoft.AspNetCore.Mvc;
using SupportSystem.Application.DTOs;
using SupportSystem.Application.Interfaces;

namespace SupportSystem.Api.Controllers;


// Controlador dedicado à consulta e envio de notificações.
// Fornece endpoints para criar, listar e marcar notificações como lidas.
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{

    // Serviço de notificações injetado via Dependency Injection.
    // Responsável pelas operações de criação, consulta e atualização de notificações.
    private readonly INotificationService _service;

 
    // Construtor com injeção do serviço de notificações.
    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    // Envia manualmente uma notificação para um usuário específico.
    // Retorna 201 Created com a notificação criada e cabeçalho Location apontando para a listagem do usuário.
    public async Task<ActionResult> SendAsync([FromBody] CreateNotificationDto dto, CancellationToken cancellationToken)
    {
        // Chama o serviço para criar/enviar a notificação.
        var notification = await _service.SendAsync(dto, cancellationToken);

        // Retorna 201 Created apontando para o endpoint que lista notificações do usuário.
        return CreatedAtAction(nameof(GetByUserAsync), new { userId = notification.UsuarioId }, notification);
    }


    // Lista notificações de um usuário.
    // Permite incluir (ou não) notificações já lidas através do parâmetro includeRead.
    public async Task<ActionResult> GetByUserAsync(Guid userId, [FromQuery] bool includeRead = false, CancellationToken cancellationToken = default)
    {
        // Busca as notificações do usuário via serviço.
        var notifications = await _service.GetByUserAsync(userId, includeRead, cancellationToken);

        // Retorna 200 OK com a lista de notificações (pode ser vazia).
        return Ok(notifications);
    }


    // Marca uma notificação específica como lida.
    // Retorna 204 No Content quando a operação for bem-sucedida ou 404 Not Found se a notificação não existir.
    public async Task<ActionResult> MarkAsReadAsync(Guid id, CancellationToken cancellationToken)
    {
        // Tenta marcar a notificação como lida via serviço.
        var updated = await _service.MarkAsReadAsync(id, cancellationToken);

        // Se a notificação não foi encontrada/atualizada, retorna 404.
        if (!updated)
        {
            return NotFound();
        }

        // Sucesso sem conteúdo.
        return NoContent();
    }
}
