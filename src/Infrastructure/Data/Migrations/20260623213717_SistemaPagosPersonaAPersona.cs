using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SistemaPagosPersonaAPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_ParticipantesViaje_ParticipanteId",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "ParticipanteId",
                table: "Pagos",
                newName: "RemitenteId");

            migrationBuilder.RenameIndex(
                name: "IX_Pagos_ParticipanteId",
                table: "Pagos",
                newName: "IX_Pagos_RemitenteId");

            migrationBuilder.AddColumn<int>(
                name: "DestinatarioId",
                table: "Pagos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_DestinatarioId",
                table: "Pagos",
                column: "DestinatarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_ParticipantesViaje_DestinatarioId",
                table: "Pagos",
                column: "DestinatarioId",
                principalTable: "ParticipantesViaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_ParticipantesViaje_RemitenteId",
                table: "Pagos",
                column: "RemitenteId",
                principalTable: "ParticipantesViaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_ParticipantesViaje_DestinatarioId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_ParticipantesViaje_RemitenteId",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_DestinatarioId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "DestinatarioId",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "RemitenteId",
                table: "Pagos",
                newName: "ParticipanteId");

            migrationBuilder.RenameIndex(
                name: "IX_Pagos_RemitenteId",
                table: "Pagos",
                newName: "IX_Pagos_ParticipanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_ParticipantesViaje_ParticipanteId",
                table: "Pagos",
                column: "ParticipanteId",
                principalTable: "ParticipantesViaje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
