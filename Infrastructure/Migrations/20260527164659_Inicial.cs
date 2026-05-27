using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "Id", "Apellido", "Direccion", "DistritoId", "EsMaster", "Nombre", "Telefono", "UsuarioId" },
                values: new object[] { 1, "Central", "Oficina Principal", 1, true, "Admin", "2222-0000", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Administradores",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
