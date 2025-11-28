using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConfeccionesAlba_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImageUploadPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1011, "permission", "products:image", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 2007, "permission", "products:image", "1dacfc02-50b8-43da-a050-b0fa556224b7" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1dacfc02-50b8-43da-a050-b0fa556224b7",
                column: "ConcurrencyStamp",
                value: "2ac2efd3-2f12-4b2b-b8a1-0cd51964193c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f4dbaa01-dcae-40a5-844e-d8a719393af4",
                column: "ConcurrencyStamp",
                value: "b50ae79b-2757-4fcd-b76d-9456f2b90ec0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2007);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1dacfc02-50b8-43da-a050-b0fa556224b7",
                column: "ConcurrencyStamp",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f4dbaa01-dcae-40a5-844e-d8a719393af4",
                column: "ConcurrencyStamp",
                value: null);
        }
    }
}
