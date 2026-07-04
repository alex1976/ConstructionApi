using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructionApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAssemblyNameAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Assemblies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Assemblies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Assemblies");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Assemblies");
        }
    }
}
