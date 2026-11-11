using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosNailsWA.BD.Migrations
{
    /// <inheritdoc />
    public partial class AdaptacionModeloDER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Turnos_TurnoId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Clientes_ClienteId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_ClienteId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_TurnoId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Adelanto",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "IdTurno",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "IdServicio",
                table: "Turnos");

            migrationBuilder.AddColumn<int>(
                name: "ReservaId",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "TurnoId",
                table: "Pagos",
                newName: "ReservaId");

            migrationBuilder.RenameColumn(
                name: "FechaPago",
                table: "Pagos",
                newName: "FechaComprobanteAdjuntado");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Hora",
                table: "Turnos",
                type: "time",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Turnos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Servicios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Duracion",
                table: "Servicios",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Servicios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Servicios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPago",
                table: "Pagos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Comprobante",
                table: "Pagos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Pagos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HorariosAtencion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosAtencion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoReserva = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImporteTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ReservaId",
                table: "Turnos",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ReservaId",
                table: "Pagos",
                column: "ReservaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteId",
                table: "Reservas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_CodigoReserva",
                table: "Reservas",
                column: "CodigoReserva",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Reservas_ReservaId",
                table: "Pagos",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Reservas_ReservaId",
                table: "Turnos",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Reservas_ReservaId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Reservas_ReservaId",
                table: "Turnos");

            migrationBuilder.DropTable(
                name: "HorariosAtencion");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_ReservaId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_ReservaId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "Comprobante",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "ReservaId",
                table: "Turnos");

            migrationBuilder.AddColumn<int>(
                name: "IdServicio",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "ReservaId",
                table: "Pagos",
                newName: "TurnoId");

            migrationBuilder.RenameColumn(
                name: "FechaComprobanteAdjuntado",
                table: "Pagos",
                newName: "FechaPago");

            migrationBuilder.AlterColumn<string>(
                name: "Hora",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<decimal>(
                name: "Adelanto",
                table: "Turnos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdCliente",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Servicios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Duracion",
                table: "Servicios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPago",
                table: "Pagos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "IdTurno",
                table: "Pagos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ClienteId",
                table: "Turnos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TurnoId",
                table: "Pagos",
                column: "TurnoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Turnos_TurnoId",
                table: "Pagos",
                column: "TurnoId",
                principalTable: "Turnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Clientes_ClienteId",
                table: "Turnos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
