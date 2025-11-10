using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConfeccionesAlba_Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeItemsToProductsAndRemoveImagesPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2007);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2008);

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1008,
                column: "ClaimValue",
                value: "products:create");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1009,
                column: "ClaimValue",
                value: "products:update");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1010,
                column: "ClaimValue",
                value: "products:delete");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2004,
                column: "ClaimValue",
                value: "products:create");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2005,
                column: "ClaimValue",
                value: "products:update");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2006,
                column: "ClaimValue",
                value: "products:delete");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1008,
                column: "ClaimValue",
                value: "items:create");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1009,
                column: "ClaimValue",
                value: "items:update");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1010,
                column: "ClaimValue",
                value: "items:delete");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2004,
                column: "ClaimValue",
                value: "items:create");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2005,
                column: "ClaimValue",
                value: "items:update");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2006,
                column: "ClaimValue",
                value: "items:delete");

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1011, "permission", "images:create", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1012, "permission", "images:delete", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 2007, "permission", "images:create", "1dacfc02-50b8-43da-a050-b0fa556224b7" },
                    { 2008, "permission", "images:delete", "1dacfc02-50b8-43da-a050-b0fa556224b7" }
                });
        }
    }
}
