using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMQ.Data.Entities;

[Table("solicitacoes_relatorio")]
public class SolicitacaoRelatorio
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = string.Empty;

    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; }

    [Column("data_processamento")]
    public DateTime? DataProcessamento { get; set; }

    [MaxLength(500)]
    [Column("observacoes")]
    public string? Observacoes { get; set; }
}