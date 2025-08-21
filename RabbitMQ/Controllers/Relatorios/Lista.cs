namespace RabbitMQ.Controllers.Relatorios;

internal static class Lista 
{
    public static List<SolicitacaoRelatorio> Relatorios = new();
}

public class SolicitacaoRelatorio
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime? ProcessedTime { get; set; }
}
