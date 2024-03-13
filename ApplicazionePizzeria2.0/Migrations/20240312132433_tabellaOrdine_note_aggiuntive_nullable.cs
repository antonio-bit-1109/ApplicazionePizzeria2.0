using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicazionePizzeria2._0.Migrations
{
    /// <inheritdoc />
    public partial class tabellaOrdine_note_aggiuntive_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NoteAggiuntive",
                table: "Ordini",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NoteAggiuntive",
                table: "Ordini",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
