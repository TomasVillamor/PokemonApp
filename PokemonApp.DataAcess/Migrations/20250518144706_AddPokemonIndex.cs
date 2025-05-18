using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonApp.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddPokemonIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_Pokemon_PokeApiId",
                table: "Pokemons",
                column: "PokeApiId",
                unique: true,
                filter: "[PokeApiId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Pokemon_PokeApiId",
                table: "Pokemons");
        }
    }
}
