namespace SupportSystem.Application.DTOs;

// Representa o feedback emitido por um cliente após o atendimento.
// DTO (Data Transfer Object) utilizado para transferir informações do feedback entre camadas da aplicação (ex.: camada de aplicação -> camada de apresentação).
public record FeedbackDto(
    Guid Id, // Identificador único do feedback
    Guid ChamadoId, // Identificador do chamado/atendimento associado ao feedback
    Guid UsuarioId, // Identificador do usuário que enviou o feedback
    int Nota, // Avaliação numérica dada pelo usuário
    string? Comentario, // Comentário textual opcional fornecido pelo usuário
    DateTime CriadoEm // Data e hora em que o feedback foi criado
);

// DTO utilizado para registrar (criar) uma avaliação de atendimento.
// Contém os dados necessários para criar um novo feedback no sistema.
// Este record representa os dados enviados por um cliente/usuário quando registra sua avaliação. 
// Pode ser usado em endpoints de API ou comandos na camada de aplicação.
public record CreateFeedbackDto(
    Guid ChamadoId, // Identificador do chamado/atendimento avaliado
    Guid UsuarioId, // Identificador do usuário que realiza a avaliação
    int Nota, // Avaliação numérica fornecida
    string? Comentario // Comentário opcional com observações sobre o atendimento
);
