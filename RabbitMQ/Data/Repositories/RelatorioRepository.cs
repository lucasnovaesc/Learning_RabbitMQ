using Microsoft.EntityFrameworkCore;
using RabbitMQ.Data.Entities;

namespace RabbitMQ.Data.Repositories;



public class RelatorioRepository : IRelatorioRepository
{
    private readonly ApplicationDbContext _context;

    public RelatorioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitacaoRelatorio> CriarAsync(SolicitacaoRelatorio solicitacao)
    {
        _context.SolicitacoesRelatorio.Add(solicitacao);
        await _context.SaveChangesAsync();
        return solicitacao;
    }

    public async Task<SolicitacaoRelatorio?> ObterPorIdAsync(Guid id)
    {
        return await _context.SolicitacoesRelatorio
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<SolicitacaoRelatorio>> ObterTodosAsync()
    {
        return await _context.SolicitacoesRelatorio
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync();
    }

    public async Task<SolicitacaoRelatorio> AtualizarAsync(SolicitacaoRelatorio solicitacao)
    {
        _context.SolicitacoesRelatorio.Update(solicitacao);
        await _context.SaveChangesAsync();
        return solicitacao;
    }

    public async Task<bool> ExisteAsync(Guid id)
    {
        return await _context.SolicitacoesRelatorio
            .AnyAsync(x => x.Id == id);
    }

    public async Task<List<SolicitacaoRelatorio>> ObterPorStatusAsync(string status)
    {
        return await _context.SolicitacoesRelatorio
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync();
    }
}