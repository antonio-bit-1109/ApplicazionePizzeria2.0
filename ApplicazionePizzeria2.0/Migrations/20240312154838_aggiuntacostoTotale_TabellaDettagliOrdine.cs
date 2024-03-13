using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicazionePizzeria2._0.Migrations
{
    /// <inheritdoc />
    public partial class aggiuntacostoTotale_TabellaDettagliOrdine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CostoTotale",
                table: "DettagliOrdini",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoTotale",
                table: "DettagliOrdini");
        }
    }
}
