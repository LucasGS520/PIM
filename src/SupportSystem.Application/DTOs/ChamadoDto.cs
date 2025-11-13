using System;
using System.Collections.Generic;
using SupportSystem.Domain.Enums;

namespace SupportSystem.Application.DTOs;

// DTO que representa um chamado com os dados relevantes para as camadas de apresentação (UI/API).
// Este record é usado para transferir informações completas de um chamado entre a aplicação e as interfaces.
public record ChamadoDto(
    // Identificador único do chamado
    Guid Id,
    // Título resumido do chamado
    string Titulo,
    // Descrição detalhada do problema ou solicitação
    string Descricao,
    // Categoria textual do chamado
    string Categoria,
    // Prioridade do chamado (Baixa, Média, Alta)
    TicketPriority Prioridade,
    // Status atual do chamado (Aberto, EmProgresso, Fechado)
    TicketStatus Status,
    // Id do usuário solicitante
    Guid SolicitanteId,
    // Id do técnico atribuído, pode ser nulo quando não atribuído
    Guid? TecnicoId,
    // Data e hora de criação do chamado
    DateTime CriadoEm,
    // Data e hora da última atualização; nulo se nunca atualizado.
    DateTime? AtualizadoEm,
    // Prazo esperado para resolução; nulo se não houver prazo definido.
    DateTime? Prazo,
    // Data e hora de encerramento; nulo se ainda não encerrado.
    DateTime? EncerradoEm
);

// DTO utilizado na criação de chamados.
// Contém os dados necessários para criar um novo chamado. 
// Alguns campos são opcionais: PalavrasChave e Anexos podem ser nulos/omissos.
public record CriarChamadoDto(
    // Título do chamado (obrigatório).
    string Titulo,
    // Descrição detalhada do chamado (obrigatório).
    string Descricao,
    // Categoria do chamado; pode ser nula/omissa.
    string? Categoria,
    // Id do solicitante que está criando o chamado.
    Guid SolicitanteId,
    // Lista de palavras-chave relacionadas ao chamado; opcional.
    IEnumerable<string>? PalavrasChave,
    // Lista de referências a anexos; opcional.
    IEnumerable<string>? Anexos
);

// DTO utilizado para atualizações de chamados por técnicos.
// Permite alterações parciais: qualquer propriedade pode ser nula indicando "sem alteração".
// Mensagem é utilizada para registrar uma nota/observação do técnico.
public record AtualizarChamadoDto(
    // Nova descrição ou null para manter a atual.
    string? Descricao,
    // Novo status do chamado ou null para manter o atual.
    TicketStatus? Status,
    // Nova prioridade ou null para manter a atual.
    TicketPriority? Prioridade,
    // Id do técnico responsável (pode ser nulo para desatribuir).
    Guid? TecnicoId,
    // Mensagem explicativa da atualização.
    string? Mensagem,
    // Lista de anexos adicionados na atualização; pode ser nulo.
    IEnumerable<string>? Anexos
);

// Representa uma entrada do histórico do chamado.
// Cada instância descreve uma ação/observação registrada no histórico do chamado.
public record HistoricoChamadoDto(
    // Identificador único da entrada de histórico.
    Guid Id,
    // Id do autor que realizou a ação.
    Guid AutorId,
    // Conteúdo da mensagem ou descrição da ação registrada.
    string Mensagem,
    // Data e hora em que a entrada foi criada.
    DateTime CriadoEm,
    // Representação textual do status capturado naquele momento.
    string StatusCapturado
);
