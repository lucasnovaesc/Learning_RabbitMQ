using RabbitMQ.Bus;
using RabbitMQ.Controllers.Relatorios;
using RabbitMQ.Data.Entities; // ✅ Referência específica
using RabbitMQ.Data.Repositories;
using SolicitacaoRelatorio = RabbitMQ.Data.Entities.SolicitacaoRelatorio;

namespace RabbitMQ.Services;

public interface IRelatorioService
{
    Task<SolicitacaoRelatorio> SolicitarRelatorioAsync(string nome);
    Task<List<SolicitacaoRelatorio>> ObterTodosRelatoriosAsync();
    Task<SolicitacaoRelatorio?> ObterRelatorioPorIdAsync(Guid id);
    Task ProcessarRelatorioAsync(Guid id, string nome);
}

public class RelatorioService : IRelatorioService
{
    private readonly IRelatorioRepository _repository;
    private readonly IPublishBus _publishBus;
    private readonly ILogger<RelatorioService> _logger;

    public RelatorioService(
        IRelatorioRepository repository,
        IPublishBus publishBus,
        ILogger<RelatorioService> logger)
    {
        _repository = repository;
        _publishBus = publishBus;
        _logger = logger;
    }

    public async Task<SolicitacaoRelatorio> SolicitarRelatorioAsync(string nome)
    {
        // ✅ Usando namespace específico
        var solicitacao = new Data.Entities.SolicitacaoRelatorio
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Status = "Pendente",
            DataCriacao = DateTime.UtcNow
        };

        var solicitacaoCriada = await _repository.CriarAsync(solicitacao);

        // Publicar evento no RabbitMQ
        var eventoSolicitacao = new RelatorioSolicitadoEvent(
            solicitacaoCriada.Id,
            solicitacaoCriada.Nome
        );

        await _publishBus.PublishAsync(eventoSolicitacao);

        _logger.LogInformation(
            "Relatório solicitado: {Id} - {Nome}",
            solicitacaoCriada.Id,
            solicitacaoCriada.Nome
        );

        return solicitacaoCriada;
    }

    public async Task<List<SolicitacaoRelatorio>> ObterTodosRelatoriosAsync()
    {
        return await _repository.ObterTodosAsync();
    }

    public async Task<SolicitacaoRelatorio?> ObterRelatorioPorIdAsync(Guid id)
    {
        return await _repository.ObterPorIdAsync(id);
    }

    public async Task ProcessarRelatorioAsync(Guid id, string nome)
    {
        var relatorio = await _repository.ObterPorIdAsync(id);

        // ✅ Correção da comparação com null
        if (relatorio is null)
        {
            _logger.LogWarning("Relatório não encontrado: {Id}", id);
            return;
        }

        // Simular processamento
        await Task.Delay(1000);

        relatorio.Status = "Completado";
        relatorio.DataProcessamento = DateTime.UtcNow;
        relatorio.Observacoes = "Processamento concluído com sucesso";

        await _repository.AtualizarAsync(relatorio);

        _logger.LogInformation(
            "Relatório processado: {Id} - {Nome}",
            id,
            nome
        );
    }
}