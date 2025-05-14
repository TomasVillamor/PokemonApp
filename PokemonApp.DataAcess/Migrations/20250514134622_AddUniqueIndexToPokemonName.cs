using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonApp.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToPokemonName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_Pokemon_Name",
                table: "Pokemons",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Pokemon_Name",
                table: "Pokemons");
        }
    }
}
