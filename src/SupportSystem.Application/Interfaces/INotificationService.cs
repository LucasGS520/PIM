using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupportSystem.Application.DTOs;


// Interface para operações relacionadas envio, recuperação e marcação das notificações:
namespace SupportSystem.Application.Interfaces
{
    // Define operações para o envio e leitura de notificações.
    // Implementações devem tratar validação básica, persistência e eventual publicação de eventos/integrações relacionados ao ciclo de vida da notificação.
    public interface INotificationService
    {
        // Envia uma nova notificação manualmente.
        Task<NotificationDto> SendAsync(CreateNotificationDto dto, CancellationToken cancellationToken);

        // Obtém notificações de um usuário.
        // Indica se as notificações já lidas devem ser incluídas no resultado.
        // Coleção somente leitura com as notificações encontradas. 
        // A ordem e paginação ficam a critério da implementação concreta.
        Task<IReadOnlyCollection<NotificationDto>> GetByUserAsync(Guid userId, bool includeRead, CancellationToken cancellationToken);

        // Marca uma notificação como lida.
        // Retorna true se a operação foi bem-sucedida (notificação encontrada e atualizada); false se não.
        Task<bool> MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken);
    }
}
