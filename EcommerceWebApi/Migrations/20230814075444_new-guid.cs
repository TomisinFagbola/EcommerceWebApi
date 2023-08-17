using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class newguid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "4c77fec0-0975-43be-88cf-5c45ab9fa764");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "92ee698f-89a7-427f-a35d-06cf266149cc");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "f9ae1fb0-e6d3-4b9c-84ef-db07645a3ff8");

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "071aebd8-1fc0-4053-a75e-4d8b8f48f517", null, "Regular", "REGULAR" },
                    { "898529ad-8897-4c2f-970b-c2671769e4f7", null, "Manager", "MANAGER" },
                    { "bcebc7ac-e13d-4bf2-b77d-f2d70ee630ee", null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "071aebd8-1fc0-4053-a75e-4d8b8f48f517");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "898529ad-8897-4c2f-970b-c2671769e4f7");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "bcebc7ac-e13d-4bf2-b77d-f2d70ee630ee");

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4c77fec0-0975-43be-88cf-5c45ab9fa764", null, "Manager", "MANAGER" },
                    { "92ee698f-89a7-427f-a35d-06cf266149cc", null, "Regular", "REGULAR" },
                    { "f9ae1fb0-e6d3-4b9c-84ef-db07645a3ff8", null, "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
