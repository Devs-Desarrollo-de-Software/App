using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisGo.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForeignKeyDestinoCalificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCalificaciones_AppDestinos_DestinoId",
                table: "AppCalificaciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_AppCalificaciones_AppDestinos_DestinoId",
                table: "AppCalificaciones",
                column: "DestinoId",
                principalTable: "AppDestinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
