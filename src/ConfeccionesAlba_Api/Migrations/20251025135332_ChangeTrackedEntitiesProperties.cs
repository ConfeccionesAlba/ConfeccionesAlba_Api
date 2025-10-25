using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfeccionesAlba_Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTrackedEntitiesProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Items",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Items",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Categories",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Categories",
                newName: "CreatedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Items",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Items",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Categories",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Categories",
                newName: "Created");
        }
    }
}
