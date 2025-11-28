using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfeccionesAlba_Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSlugPropertyFromCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1dacfc02-50b8-43da-a050-b0fa556224b7",
                column: "ConcurrencyStamp",
                value: "79970079-cbfd-4a65-9aee-71b3d20446bc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f4dbaa01-dcae-40a5-844e-d8a719393af4",
                column: "ConcurrencyStamp",
                value: "0414e7ff-3b70-44a8-bcc5-13ae18b3887b");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Categories",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

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
    }
}
