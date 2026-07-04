using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructionApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CatalogCode = table.Column<string>(type: "TEXT", nullable: false),
                    CatalogDescription = table.Column<string>(type: "TEXT", nullable: false),
                    CatalogAuthor = table.Column<string>(type: "TEXT", nullable: false),
                    CatalogVersion = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Subcategory = table.Column<string>(type: "TEXT", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    SafetyPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    LabourPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaterialPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    EquipmentPercentage = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceLists", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceLists");
        }
    }
}
