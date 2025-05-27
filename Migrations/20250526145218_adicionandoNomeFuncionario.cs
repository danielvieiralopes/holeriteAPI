using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoleriteApi.Migrations
{
    /// <inheritdoc />
    public partial class adicionandoNomeFuncionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomeFuncionario",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeFuncionario",
                table: "AspNetUsers");
        }
    }
}
