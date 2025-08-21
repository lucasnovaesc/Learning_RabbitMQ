using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Services;
using RabbitMQ.Data.Entities;

namespace RabbitMQ.Controllers;

internal static class ApiEndpoints
{
    public static void AddApiEndpoints(this WebApplication app)
    {
        var relatorios = app.MapGroup("/api/relatorios")
            .WithTags("Relatórios");

        relatorios.MapPost("/solicitar/{nome}", async (
            string nome,
            [FromServices] IRelatorioService relatorioService,
            CancellationToken ct = default) =>
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return Results.BadRequest("Nome do relatório é obrigatório");
            }

            var solicitacao = await relatorioService.SolicitarRelatorioAsync(nome);
            return Results.Created($"/api/relatorios/{solicitacao.Id}", solicitacao);
        })
        .WithSummary("Solicita um novo relatório")
        .WithDescription("Cria uma nova solicitação de relatório e envia para processamento assíncrono");

        relatorios.MapGet("/", async (
            [FromServices] IRelatorioService relatorioService) =>
        {
            var listaRelatorios = await relatorioService.ObterTodosRelatoriosAsync();
            return Results.Ok(listaRelatorios);
        })
        .WithSummary("Lista todos os relatórios")
        .WithDescription("Retorna todas as solicitações de relatório com seus status");

        relatorios.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] IRelatorioService relatorioService) =>
        {
            var relatorio = await relatorioService.ObterRelatorioPorIdAsync(id);
            return relatorio is null ? Results.NotFound() : Results.Ok(relatorio);
        })
        .WithSummary("Obtém relatório por ID")
        .WithDescription("Retorna os detalhes de uma solicitação específica");
    }
}