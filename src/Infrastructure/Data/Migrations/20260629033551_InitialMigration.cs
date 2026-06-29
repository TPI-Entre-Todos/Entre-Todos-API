using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Rol = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Viajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Moneda = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viajes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Leida = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ViajeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioInvitadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailInvitado = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitaciones_Usuarios_UsuarioInvitadorId",
                        column: x => x.UsuarioInvitadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitaciones_Viajes_ViajeId",
                        column: x => x.ViajeId,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantesViaje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    ViajeId = table.Column<int>(type: "INTEGER", nullable: false),
                    EsOrganizador = table.Column<bool>(type: "INTEGER", nullable: false),
                    SaldoTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantesViaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantesViaje_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantesViaje_Viajes_ViajeId",
                        column: x => x.ViajeId,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ViajeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParticipanteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoDivision = table.Column<int>(type: "INTEGER", nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", nullable: true),
                    Comprobante = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gastos_ParticipantesViaje_ParticipanteId",
                        column: x => x.ParticipanteId,
                        principalTable: "ParticipantesViaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gastos_Viajes_ViajeId",
                        column: x => x.ViajeId,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ViajeId = table.Column<int>(type: "INTEGER", nullable: false),
                    RemitenteId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinatarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metodo = table.Column<string>(type: "TEXT", nullable: false),
                    Comprobante = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_ParticipantesViaje_DestinatarioId",
                        column: x => x.DestinatarioId,
                        principalTable: "ParticipantesViaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_ParticipantesViaje_RemitenteId",
                        column: x => x.RemitenteId,
                        principalTable: "ParticipantesViaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_Viajes_ViajeId",
                        column: x => x.ViajeId,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesGasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GastoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParticipanteId = table.Column<int>(type: "INTEGER", nullable: false),
                    MontoDebe = table.Column<decimal>(type: "TEXT", nullable: false),
                    MontoPagado = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesGasto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesGasto_Gastos_GastoId",
                        column: x => x.GastoId,
                        principalTable: "Gastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesGasto_ParticipantesViaje_ParticipanteId",
                        column: x => x.ParticipanteId,
                        principalTable: "ParticipantesViaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagosDetallesGasto",
                columns: table => new
                {
                    DetallesPagadosId = table.Column<int>(type: "INTEGER", nullable: false),
                    PagoId = table.Column<int>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "FechaRegistro", "Nombre", "Password", "Rol" },
                values: new object[] { 1, "admin@entretodos.com", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", "Admin123!", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesGasto_GastoId",
                table: "DetallesGasto",
                column: "GastoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesGasto_ParticipanteId",
                table: "DetallesGasto",
                column: "ParticipanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_ParticipanteId",
                table: "Gastos",
                column: "ParticipanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_ViajeId",
                table: "Gastos",
                column: "ViajeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_UsuarioInvitadorId",
                table: "Invitaciones",
                column: "UsuarioInvitadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_ViajeId",
                table: "Invitaciones",
                column: "ViajeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_DestinatarioId",
                table: "Pagos",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_RemitenteId",
                table: "Pagos",
                column: "RemitenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ViajeId",
                table: "Pagos",
                column: "ViajeId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosDetallesGasto_PagoId",
                table: "PagosDetallesGasto",
                column: "PagoId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesViaje_UsuarioId",
                table: "ParticipantesViaje",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesViaje_ViajeId",
                table: "ParticipantesViaje",
                column: "ViajeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invitaciones");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "PagosDetallesGasto");

            migrationBuilder.DropTable(
                name: "DetallesGasto");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Gastos");

            migrationBuilder.DropTable(
                name: "ParticipantesViaje");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Viajes");
        }
    }
}
