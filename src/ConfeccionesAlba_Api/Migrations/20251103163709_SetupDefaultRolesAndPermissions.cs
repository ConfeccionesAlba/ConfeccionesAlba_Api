using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConfeccionesAlba_Api.Migrations
{
    /// <inheritdoc />
    public partial class SetupDefaultRolesAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1dacfc02-50b8-43da-a050-b0fa556224b7", null, "Publisher", "PUBLISHER" },
                    { "f4dbaa01-dcae-40a5-844e-d8a719393af4", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1001, "permission", "users:read", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1002, "permission", "users:create", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1003, "permission", "users:update", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1004, "permission", "users:delete", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1005, "permission", "category:create", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1006, "permission", "category:update", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1007, "permission", "category:delete", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1008, "permission", "item:create", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1009, "permission", "item:update", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 1010, "permission", "item:delete", "f4dbaa01-dcae-40a5-844e-d8a719393af4" },
                    { 2001, "permission", "category:create", "1dacfc02-50b8-43da-a050-b0fa556224b7" },
                    { 2002, "permission", "category:update", "1dacfc02-50b8-43da-a050-b0fa556224b7" },
                    { 2003, "permission", "category:delete", "1dacfc02-50b8-43da-a050-b0fa556224b7" },
                    { 2004, "permission", "item:create", "1dacfc02-50b8-43da-a050-b0fa556224b7" },
                    { 2005, "permission", "item:update", "1dacfc02-50b8-43da-a050-b0fa556224b7" },
                    { 2006, "permission", "item:delete", "1dacfc02-50b8-43da-a050-b0fa556224b7" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2002);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2003);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2004);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2005);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2006);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1dacfc02-50b8-43da-a050-b0fa556224b7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f4dbaa01-dcae-40a5-844e-d8a719393af4");
        }
    }
}
