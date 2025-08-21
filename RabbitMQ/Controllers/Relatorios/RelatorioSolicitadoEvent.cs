namespace RabbitMQ.Controllers.Relatorios;

public class RelatorioSolicitadoEvent
{
    public Guid ID { get; set; }
    public string Name { get; set; }

    public RelatorioSolicitadoEvent(Guid id, string name)
    {
        ID = id;
        Name = name;
    }
}