using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationWithIdentity.Migrations
{
    public partial class AddedRolesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8984597c-7985-4748-910f-903f1664b120", "10b5632a-d138-481d-9d1f-82164a71b2e9", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ad78effc-5552-4425-b8f7-a3881adf1edb", "b750bcb2-e528-4f2b-ac86-cf0b55fd3a1c", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d25fdd45-81ae-4d4d-977b-69ddde5232e9", "4929db9d-367e-48b6-b766-3eea0692299f", "Customer", "CUSTOMER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8984597c-7985-4748-910f-903f1664b120");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad78effc-5552-4425-b8f7-a3881adf1edb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d25fdd45-81ae-4d4d-977b-69ddde5232e9");
        }
    }
}
