using System;
using System.Collections.Generic;

namespace SupportSystem.Application.AI
{
    // Representa o resultado consolidado das análises de IA para um chamado.
    // Contém a categoria e prioridade sugeridas e uma lista de artigos relacionados.
    public sealed record AssistantResult(
        string CategoriaSugerida, // Categoria sugerida pelo assistente de IA (ex.: "Rede", "Software")
        string PrioridadeSugerida, // Prioridade sugerida pelo assistente (ex.: "Alta", "Normal")
        IReadOnlyCollection<Guid> ArtigosRelacionados // Coleção imutável de IDs de artigos relacionados sugeridos pela IA
    );
}
