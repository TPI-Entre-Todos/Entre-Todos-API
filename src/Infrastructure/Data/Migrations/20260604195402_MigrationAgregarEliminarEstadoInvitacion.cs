using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationAgregarEliminarEstadoInvitacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoInvitacion",
                table: "ParticipantesViaje");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoInvitacion",
                table: "ParticipantesViaje",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
