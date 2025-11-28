using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfeccionesAlba_Api.Migrations
{
    /// <inheritdoc />
    public partial class SetProductImageComplexTypeNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image_Url",
                table: "Products",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Image_Name",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1dacfc02-50b8-43da-a050-b0fa556224b7",
                column: "ConcurrencyStamp",
                value: "dc8bafd2-499f-450d-a8c7-8ff6701b62d8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f4dbaa01-dcae-40a5-844e-d8a719393af4",
                column: "ConcurrencyStamp",
                value: "ad1e0f10-30e5-4432-95fa-51086d4f19cc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image_Url",
                table: "Products",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image_Name",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

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
    }
}
