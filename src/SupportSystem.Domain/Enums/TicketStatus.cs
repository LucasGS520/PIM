namespace SupportSystem.Domain.Enums;

/// <summary>
/// Representa os estados possíveis de um chamado.
/// </summary>
public enum TicketStatus
{
    Aberto = 0,
    EmAndamento = 1,
    AguardandoUsuario = 2,
    Resolvido = 3,
    Fechado = 4,
    Reaberto = 5
}
