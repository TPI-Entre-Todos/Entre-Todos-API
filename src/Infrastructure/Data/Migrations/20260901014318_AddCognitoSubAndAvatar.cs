using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCognitoSubAndAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // El admin inicial (Id = 1) deja de estar gestionado como seed de EF, pero la fila
            // se conserva a propósito: cuando ese admin se loguee por Cognito con el mismo
            // email, GetOrCreateFromToken lo adopta vinculándole el CognitoSub, y así mantiene
            // su rol Admin y sus relaciones. Por eso NO se borra el registro acá.

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Usuarios");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Usuarios",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CognitoSub",
                table: "Usuarios",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CognitoSub",
                table: "Usuarios",
                column: "CognitoSub",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CognitoSub",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CognitoSub",
                table: "Usuarios");

            // Nullable, a diferencia del esquema original: al revertir ya existen usuarios
            // creados vía Cognito que nunca tuvieron contraseña local.
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
