namespace SupportSystem.Application.AI;

/// <summary>
/// Representa o resultado consolidado das análises de IA para um chamado.
/// </summary>
public sealed record AssistantResult(
    string CategoriaSugerida,
    string PrioridadeSugerida,
    IReadOnlyCollection<Guid> ArtigosRelacionados
);
