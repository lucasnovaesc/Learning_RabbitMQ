using RabbitMQ.Data.Entities;

namespace RabbitMQ.Data.Repositories;

public interface IRelatorioRepository
{
    Task<SolicitacaoRelatorio> CriarAsync(SolicitacaoRelatorio solicitacao);
    Task<SolicitacaoRelatorio?> ObterPorIdAsync(Guid id);
    Task<List<SolicitacaoRelatorio>> ObterTodosAsync();
    Task<SolicitacaoRelatorio> AtualizarAsync(SolicitacaoRelatorio solicitacao);
    Task<bool> ExisteAsync(Guid id);
    Task<List<SolicitacaoRelatorio>> ObterPorStatusAsync(string status);
}