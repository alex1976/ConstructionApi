using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructionApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Phases",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Phases");
        }
    }
}
