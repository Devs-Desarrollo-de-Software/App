using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurisGo.Migrations
{
    /// <inheritdoc />
    public partial class AddMetricasApiExternaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppMetricasApiExterna",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreApi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MetodoHttp = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ParametrosConsulta = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CodigoEstadoHttp = table.Column<int>(type: "int", nullable: false),
                    TiempoRespuestaMs = table.Column<long>(type: "bigint", nullable: false),
                    Exitosa = table.Column<bool>(type: "bit", nullable: false),
                    MensajeError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CantidadResultados = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMetricasApiExterna", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetricasApiExterna_CreationTime",
                table: "AppMetricasApiExterna",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_MetricasApiExterna_Exitosa",
                table: "AppMetricasApiExterna",
                column: "Exitosa");

            migrationBuilder.CreateIndex(
                name: "IX_MetricasApiExterna_NombreApi",
                table: "AppMetricasApiExterna",
                column: "NombreApi");

            migrationBuilder.CreateIndex(
                name: "IX_MetricasApiExterna_NombreApi_CreationTime",
                table: "AppMetricasApiExterna",
                columns: new[] { "NombreApi", "CreationTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppMetricasApiExterna");
        }
    }
}
