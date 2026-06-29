using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDetalleGasto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MontoIndividual",
                table: "DetallesGasto",
                newName: "MontoPagado");

            migrationBuilder.AddColumn<decimal>(
                name: "MontoDebe",
                table: "DetallesGasto",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontoDebe",
                table: "DetallesGasto");

            migrationBuilder.RenameColumn(
                name: "MontoPagado",
                table: "DetallesGasto",
                newName: "MontoIndividual");
        }
    }
}
