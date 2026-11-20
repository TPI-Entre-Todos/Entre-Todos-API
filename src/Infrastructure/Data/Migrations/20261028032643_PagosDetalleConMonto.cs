using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class PagosDetalleConMonto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PagosDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PagoId = table.Column<int>(type: "int", nullable: false),
                    DetalleGastoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosDetalle_DetallesGasto_DetalleGastoId",
                        column: x => x.DetalleGastoId,
                        principalTable: "DetallesGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagosDetalle_Pagos_PagoId",
                        column: x => x.PagoId,
                        principalTable: "Pagos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PagosDetalle_DetalleGastoId",
                table: "PagosDetalle",
                column: "DetalleGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosDetalle_PagoId",
                table: "PagosDetalle",
                column: "PagoId");

            // Pagos existentes: la tabla vieja no guardaba cuánto fue a cada detalle.
            // Se reparte el monto del pago en proporción a lo ya pagado de cada detalle
            // (con un solo detalle por pago queda exacto: Monto = Pago.Monto).
            migrationBuilder.Sql(@"
                INSERT INTO PagosDetalle (PagoId, DetalleGastoId, Monto)
                SELECT pdg.PagoId, pdg.DetallesPagadosId,
                       CASE WHEN tot.Total > 0 THEN p.Monto * d.MontoPagado / tot.Total ELSE 0 END
                FROM PagosDetallesGasto pdg
                JOIN Pagos p ON p.Id = pdg.PagoId
                JOIN DetallesGasto d ON d.Id = pdg.DetallesPagadosId
                JOIN (
                    SELECT x.PagoId, SUM(dd.MontoPagado) AS Total
                    FROM PagosDetallesGasto x
                    JOIN DetallesGasto dd ON dd.Id = x.DetallesPagadosId
                    GROUP BY x.PagoId
                ) tot ON tot.PagoId = pdg.PagoId;");

            migrationBuilder.DropTable(
                name: "PagosDetallesGasto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PagosDetallesGasto",
                columns: table => new
                {
                    DetallesPagadosId = table.Column<int>(type: "int", nullable: false),
                    PagoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosDetallesGasto", x => new { x.DetallesPagadosId, x.PagoId });
                    table.ForeignKey(
                        name: "FK_PagosDetallesGasto_DetallesGasto_DetallesPagadosId",
                        column: x => x.DetallesPagadosId,
                        principalTable: "DetallesGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagosDetallesGasto_Pagos_PagoId",
                        column: x => x.PagoId,
                        principalTable: "Pagos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PagosDetallesGasto_PagoId",
                table: "PagosDetallesGasto",
                column: "PagoId");

            migrationBuilder.Sql(@"
                INSERT INTO PagosDetallesGasto (PagoId, DetallesPagadosId)
                SELECT PagoId, DetalleGastoId FROM PagosDetalle;");

            migrationBuilder.DropTable(
                name: "PagosDetalle");
        }
    }
}
