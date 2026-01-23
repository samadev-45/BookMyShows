using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookMyShow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MovieShows",
                keyColumn: "Id",
                keyValue: new Guid("2deef50a-84f6-40c8-afb2-87f68365fc17"));

            migrationBuilder.DeleteData(
                table: "MovieShows",
                keyColumn: "Id",
                keyValue: new Guid("ac081390-c047-48b5-ab0d-d14ec91b146c"));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Seats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "BookingStatus",
                table: "Bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "MovieShows",
                columns: new[] { "Id", "MovieTitle", "ScreenNumber", "ShowTime", "TotalSeats" },
                values: new object[,]
                {
                    { new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "Spider-Man: No Way Home", "Screen 2", new DateTime(2026, 1, 24, 16, 56, 45, 134, DateTimeKind.Utc).AddTicks(5823), 75 },
                    { new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "Avengers: Endgame", "Screen 1", new DateTime(2026, 1, 23, 16, 56, 45, 134, DateTimeKind.Utc).AddTicks(5808), 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MovieShows",
                keyColumn: "Id",
                keyValue: new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"));

            migrationBuilder.DeleteData(
                table: "MovieShows",
                keyColumn: "Id",
                keyValue: new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Seats",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "BookingStatus",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "MovieShows",
                columns: new[] { "Id", "MovieTitle", "ScreenNumber", "ShowTime", "TotalSeats" },
                values: new object[,]
                {
                    { new Guid("2deef50a-84f6-40c8-afb2-87f68365fc17"), "Spider-Man: No Way Home", "Screen 2", new DateTime(2026, 1, 24, 9, 40, 42, 2, DateTimeKind.Utc).AddTicks(1254), 75 },
                    { new Guid("ac081390-c047-48b5-ab0d-d14ec91b146c"), "Avengers: Endgame", "Screen 1", new DateTime(2026, 1, 23, 9, 40, 42, 2, DateTimeKind.Utc).AddTicks(1222), 100 }
                });
        }
    }
}
