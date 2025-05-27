using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoleriteApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDataNascimentoAndPrecisaTrocarSenhaAndUsuarioAtivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "PrecisaTrocarSenha",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsuarioAtivo",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PrecisaTrocarSenha",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UsuarioAtivo",
                table: "AspNetUsers");
        }
    }
}
