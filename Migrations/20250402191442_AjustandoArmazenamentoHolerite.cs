using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoleriteApi.Migrations
{
    /// <inheritdoc />
    public partial class AjustandoArmazenamentoHolerite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataReferencia",
                table: "Holerites",
                newName: "DataUpload");

            migrationBuilder.AddColumn<int>(
                name: "AnoReferencia",
                table: "Holerites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MesReferencia",
                table: "Holerites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoHolerite",
                table: "Holerites",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnoReferencia",
                table: "Holerites");

            migrationBuilder.DropColumn(
                name: "MesReferencia",
                table: "Holerites");

            migrationBuilder.DropColumn(
                name: "TipoHolerite",
                table: "Holerites");

            migrationBuilder.RenameColumn(
                name: "DataUpload",
                table: "Holerites",
                newName: "DataReferencia");
        }
    }
}
