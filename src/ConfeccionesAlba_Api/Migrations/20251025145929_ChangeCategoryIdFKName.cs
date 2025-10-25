using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfeccionesAlba_Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCategoryIdFKName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_FK_Item_Category_CategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_FK_Item_Category_CategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FK_Item_Category_CategoryId",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryId",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "FK_Item_Category_CategoryId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_FK_Item_Category_CategoryId",
                table: "Items",
                column: "FK_Item_Category_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_FK_Item_Category_CategoryId",
                table: "Items",
                column: "FK_Item_Category_CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
