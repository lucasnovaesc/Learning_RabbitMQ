using MassTransit;
using RabbitMQ.Controllers.Relatorios;
using RabbitMQ.Services;

namespace RabbitMQ.Bus;

public sealed class RelatorioSolicitadoEventConsumer : IConsumer<RelatorioSolicitadoEvent> // ✅ Mudou para public
{
    private readonly IRelatorioService _relatorioService;
    private readonly ILogger<RelatorioSolicitadoEventConsumer> _logger;

    public RelatorioSolicitadoEventConsumer(
        IRelatorioService relatorioService,
        ILogger<RelatorioSolicitadoEventConsumer> logger)
    {
        _relatorioService = relatorioService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RelatorioSolicitadoEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Processando relatório: {ID}, Nome: {Name}", 
            message.ID, 
            message.Name
        );

        try
        {
            await _relatorioService.ProcessarRelatorioAsync(message.ID, message.Name);
            
            _logger.LogInformation(
                "Relatório {ID} com nome {Name} foi processado com sucesso", 
                message.ID, 
                message.Name
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao processar relatório {ID}: {Error}", 
                message.ID, 
                ex.Message
            );
            
            throw; // Re-throw para MassTransit lidar com retry
        }
    }
}