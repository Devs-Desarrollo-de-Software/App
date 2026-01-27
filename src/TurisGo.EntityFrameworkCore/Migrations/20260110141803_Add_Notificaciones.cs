using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisGo.Migrations
{
    /// <inheritdoc />
    public partial class Add_Notificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppNotificaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnviadaPorMail = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEnvioMail = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NombreDestino = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNotificaciones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppFavoritos_DestinoId",
                table: "AppFavoritos",
                column: "DestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_CreationTime",
                table: "AppNotificaciones",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_EnviadaPorMail",
                table: "AppNotificaciones",
                column: "EnviadaPorMail",
                filter: "[EnviadaPorMail] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UserId",
                table: "AppNotificaciones",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UserId_DestinoId",
                table: "AppNotificaciones",
                columns: new[] { "UserId", "DestinoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UserId_Leida",
                table: "AppNotificaciones",
                columns: new[] { "UserId", "Leida" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppFavoritos_AppDestinos_DestinoId",
                table: "AppFavoritos",
                column: "DestinoId",
                principalTable: "AppDestinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppFavoritos_AppDestinos_DestinoId",
                table: "AppFavoritos");

            migrationBuilder.DropTable(
                name: "AppNotificaciones");

            migrationBuilder.DropIndex(
                name: "IX_AppFavoritos_DestinoId",
                table: "AppFavoritos");
        }
    }
}
