using System;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.DTOs;

// Conjunto de DTOs relacionados a notificações usados pela camada de aplicação.
// Representa uma notificação comunicada a um usuário.
public record NotificacaoDto(
    Guid Id, // Identificador único da notificação
    Guid UsuarioId, // Identificador do usuário destinatário da notificação
    string Mensagem, // Conteúdo textual da notificação
    NotificationType Tipo, // Tipo da notificação
    bool Lido, // Indica se a notificação já foi marcada como lida
    DateTime CriadoEm // Data e hora em que a notificação foi criada
);


// DTO utilizado para envio manual de notificações.
// Este DTO contém apenas os campos necessários para criar/enfileirar uma notificação a partir da camada de aplicação
public record CriarNotificacaoDto(
    Guid UsuarioId, // Identificador do usuário que receberá a notificação
    string Mensagem, // Conteúdo da notificação a ser enviada
    NotificationType Tipo, // Tipo da notificação a ser criada
    Guid? ChamadoId // Id opcional do chamado relacionado à notificação
);
