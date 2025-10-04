using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisGo.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFechaUltimaActualizacionFromDestino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaUltimaActualizacion",
                table: "AppDestinos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "AppDestinos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
