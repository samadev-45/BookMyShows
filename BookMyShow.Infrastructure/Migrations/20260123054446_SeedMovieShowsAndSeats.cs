using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookMyShow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedMovieShowsAndSeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "MovieShows",
                keyColumn: "Id",
                keyValue: new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"),
                column: "ShowTime",
                value: new DateTime(2026, 1, 24, 16, 56, 45, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "MovieShows",
                keyColumn: "Id",
                keyValue: new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"),
                column: "ShowTime",
                value: new DateTime(2026, 1, 23, 16, 56, 45, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "Id", "BookingId", "ExpiryTime", "MovieShowId", "SeatNumber", "Status" },
                values: new object[,]
                {
                    { new Guid("018e9370-bc81-460c-a203-7469f47844d0"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "C2", "Available" },
                    { new Guid("25bd6cb9-151b-4af4-896d-3d5234e8a026"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "A2", "Available" },
                    { new Guid("35c28339-ae97-48f7-a72a-bd1435fcfe43"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "D1", "Available" },
                    { new Guid("37cc5866-1bc0-4be9-8480-50b64e190a47"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "A1", "Available" },
                    { new Guid("42fd7d61-c0af-43d9-90b6-7b801dc68985"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "C3", "Available" },
                    { new Guid("4b1f7d23-4101-49f9-b0d1-126c33fc9c77"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "B2", "Available" },
                    { new Guid("4ff99623-beac-4d44-b72d-dccf39c25c02"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "A2", "Available" },
                    { new Guid("52612dd3-a813-42c5-a8bc-4bdac9d3895a"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "D2", "Available" },
                    { new Guid("57f1c2f3-ddab-4b3d-88e5-00af99aeb1de"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "C1", "Available" },
                    { new Guid("795e3a99-e18a-4785-80d5-3322b04d37e2"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "D2", "Available" },
                    { new Guid("8f4dd792-c487-4837-a1f0-440f6c2b3135"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "C3", "Available" },
                    { new Guid("9014dc60-7617-48df-8dc4-73a8270105e9"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "B2", "Available" },
                    { new Guid("9045ded9-96b2-4da1-8f16-3ad36b639993"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "B1", "Available" },
                    { new Guid("9aaed74c-ffbf-482a-a4bf-65ff0e495170"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "B1", "Available" },
                    { new Guid("a35b9587-3128-4af6-ab6e-506b1c29bde2"), null, null, new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"), "A3", "Available" },
                    { new Guid("b17d1910-1380-49f3-b7f9-da71e68f6ec0"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "D1", "Available" },
                    { new Guid("b1e1c4a4-080b-4e9b-bd52-c71122d57932"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "A3", "Available" },
                    { new Guid("cbeed942-ded8-49e3-9cd1-ff9afc0a7c35"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "C2", "Available" },
                    { new Guid("d7979b07-b4a4-4f74-85b1-f74ceb3ad1fc"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "A1", "Available" },
                    { new Guid("dab26cb5-b082-4f22-b967-94cbe803267c"), null, null, new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"), "C1", "Available" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("018e9370-bc81-460c-a203-7469f47844d0"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("25bd6cb9-151b-4af4-896d-3d5234e8a026"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("35c28339-ae97-48f7-a72a-bd1435fcfe43"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("37cc5866-1bc0-4be9-8480-50b64e190a47"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("42fd7d61-c0af-43d9-90b6-7b801dc68985"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("4b1f7d23-4101-49f9-b0d1-126c33fc9c77"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("4ff99623-beac-4d44-b72d-dccf39c25c02"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("52612dd3-a813-42c5-a8bc-4bdac9d3895a"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("57f1c2f3-ddab-4b3d-88e5-00af99aeb1de"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("795e3a99-e18a-4785-80d5-3322b04d37e2"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("8f4dd792-c487-4837-a1f0-440f6c2b3135"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("9014dc60-7617-48df-8dc4-73a8270105e9"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("9045ded9-96b2-4da1-8f16-3ad36b639993"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("9aaed74c-ffbf-482a-a4bf-65ff0e495170"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("a35b9587-3128-4af6-ab6e-506b1c29bde2"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("b17d1910-1380-49f3-b7f9-da71e68f6ec0"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("b1e1c4a4-080b-4e9b-bd52-c71122d57932"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("cbeed942-ded8-49e3-9cd1-ff9afc0a7c35"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("d7979b07-b4a4-4f74-85b1-f74ceb3ad1fc"));

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: new Guid("dab26cb5-b082-4f22-b967-94cbe803267c"));

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

            migrationBuilder.UpdateData(
                table: "MovieShows",
                keyColumn: "Id",
                keyValue: new Guid("67d35d72-2fdc-4c6e-a5b1-6a2eb7503c16"),
                column: "ShowTime",
                value: new DateTime(2026, 1, 24, 16, 56, 45, 134, DateTimeKind.Utc).AddTicks(5823));

            migrationBuilder.UpdateData(
                table: "MovieShows",
                keyColumn: "Id",
                keyValue: new Guid("f525ebb5-623f-4779-bbaa-79557d2b909d"),
                column: "ShowTime",
                value: new DateTime(2026, 1, 23, 16, 56, 45, 134, DateTimeKind.Utc).AddTicks(5808));
        }
    }
}
