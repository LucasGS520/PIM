using SupportSystem.Domain.Entities;

namespace SupportSystem.Application.Interfaces
{
    // Define um mecanismo para envio de notificações automáticas.
    // Implementações desta interface devem encapsular a lógica de transporte (ex.: envio de e-mail, SMS, push notification) e garantir tratamento de erros/retries.
    // Usar esta abstração para manter camadas de aplicação independentes do provedor.
    public interface INotificationDispatcher
    {
        // Envia uma mensagem informativa relacionada a um "Ticket" para as partes interessadas.
        Task DispatchTicketUpdateAsync(Ticket ticket, string message, CancellationToken cancellationToken);
    }
}
