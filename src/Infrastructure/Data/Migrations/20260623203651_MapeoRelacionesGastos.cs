using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MapeoRelacionesGastos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesGasto_ParticipantesViaje_ParticipanteId",
                table: "DetallesGasto");

            migrationBuilder.DropForeignKey(
                name: "FK_Gastos_ParticipantesViaje_ParticipanteId",
                table: "Gastos");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesGasto_ParticipantesViaje_ParticipanteId",
                table: "DetallesGasto",
                column: "ParticipanteId",
                principalTable: "ParticipantesViaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gastos_ParticipantesViaje_ParticipanteId",
                table: "Gastos",
                column: "ParticipanteId",
                principalTable: "ParticipantesViaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesGasto_ParticipantesViaje_ParticipanteId",
                table: "DetallesGasto");

            migrationBuilder.DropForeignKey(
                name: "FK_Gastos_ParticipantesViaje_ParticipanteId",
                table: "Gastos");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesGasto_ParticipantesViaje_ParticipanteId",
                table: "DetallesGasto",
                column: "ParticipanteId",
                principalTable: "ParticipantesViaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gastos_ParticipantesViaje_ParticipanteId",
                table: "Gastos",
                column: "ParticipanteId",
                principalTable: "ParticipantesViaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
