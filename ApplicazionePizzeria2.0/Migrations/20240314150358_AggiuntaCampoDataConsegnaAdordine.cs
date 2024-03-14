using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicazionePizzeria2._0.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntaCampoDataConsegnaAdordine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataDellaConsegna",
                table: "Ordini",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataDellaConsegna",
                table: "Ordini");
        }
    }
}
