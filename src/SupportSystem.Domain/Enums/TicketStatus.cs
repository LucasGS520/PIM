namespace SupportSystem.Domain.Enums;

// Representa os estados possíveis de um chamado.
public enum TicketStatus
{
    Aberto = 0,
    EmAndamento = 1,
    AguardandoUsuario = 2,
    Resolvido = 3,
    Fechado = 4,
    Reaberto = 5
}
