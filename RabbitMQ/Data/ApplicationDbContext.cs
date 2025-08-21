using Microsoft.EntityFrameworkCore;
using RabbitMQ.Data.Entities;

namespace RabbitMQ.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SolicitacaoRelatorio> SolicitacoesRelatorio { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SolicitacaoRelatorio>(entity =>
        {
            entity.HasIndex(e => e.Status)
                  .HasDatabaseName("ix_solicitacoes_relatorio_status");

            entity.HasIndex(e => e.DataCriacao)
                  .HasDatabaseName("ix_solicitacoes_relatorio_data_criacao");

            entity.Property(e => e.Id)
                  .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.DataCriacao)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}