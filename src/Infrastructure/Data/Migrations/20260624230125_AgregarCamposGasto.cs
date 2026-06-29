using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposGasto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Gastos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comprobante",
                table: "Gastos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoDivision",
                table: "Gastos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "Comprobante",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "TipoDivision",
                table: "Gastos");
        }
    }
}
