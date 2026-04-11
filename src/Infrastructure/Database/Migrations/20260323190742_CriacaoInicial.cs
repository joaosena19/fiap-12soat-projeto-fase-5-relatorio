using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "resultado_diagramas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    analise_diagrama_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_analise = table.Column<string>(type: "text", nullable: false),
                    analise_resultado = table.Column<string>(type: "jsonb", nullable: true),
                    relatorios = table.Column<string>(type: "jsonb", nullable: false),
                    erros = table.Column<string>(type: "jsonb", nullable: false),
                    data_criacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultado_diagramas", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_resultado_diagramas_analise_diagrama_id",
                table: "resultado_diagramas",
                column: "analise_diagrama_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resultado_diagramas");
        }
    }
}
