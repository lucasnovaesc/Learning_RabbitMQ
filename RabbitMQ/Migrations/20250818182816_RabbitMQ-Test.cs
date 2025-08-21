using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RabbitMQ.Migrations
{
    /// <inheritdoc />
    public partial class RabbitMQTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "solicitacoes_relatorio",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    data_processamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitacoes_relatorio", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_solicitacoes_relatorio_data_criacao",
                table: "solicitacoes_relatorio",
                column: "data_criacao");

            migrationBuilder.CreateIndex(
                name: "ix_solicitacoes_relatorio_status",
                table: "solicitacoes_relatorio",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "solicitacoes_relatorio");
        }
    }
}
