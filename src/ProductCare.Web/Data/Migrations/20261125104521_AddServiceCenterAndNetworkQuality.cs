using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCare.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceCenterAndNetworkQuality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentContentType",
                table: "ServiceRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                table: "ServiceRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentOriginalFileName",
                table: "ServiceRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CampusLocation",
                table: "ServiceRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEscalated",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionProofFileName",
                table: "ServiceRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceCenterId",
                table: "ServiceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SlaDeadlineUtc",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAtUtc",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NetworkQualityReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CampusZone = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NetworkType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DownloadSpeedMbps = table.Column<double>(type: "float", nullable: false),
                    LatencyMs = table.Column<int>(type: "int", nullable: false),
                    IsClientMeasured = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MeasuredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkQualityReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkQualityReports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Locality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SupportedBrands = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SupportedCategories = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    OpeningHours = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MapUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCenters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ServiceCenterId",
                table: "ServiceRequests",
                column: "ServiceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserId_IsArchived",
                table: "Products",
                columns: new[] { "UserId", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "IX_NetworkQualityReports_CampusZone",
                table: "NetworkQualityReports",
                column: "CampusZone");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkQualityReports_MeasuredAtUtc",
                table: "NetworkQualityReports",
                column: "MeasuredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkQualityReports_UserId",
                table: "NetworkQualityReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCenters_City",
                table: "ServiceCenters",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCenters_IsActive",
                table: "ServiceCenters",
                column: "IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_ServiceCenters_ServiceCenterId",
                table: "ServiceRequests",
                column: "ServiceCenterId",
                principalTable: "ServiceCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_ServiceCenters_ServiceCenterId",
                table: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "NetworkQualityReports");

            migrationBuilder.DropTable(
                name: "ServiceCenters");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_ServiceCenterId",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_Products_UserId_IsArchived",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AttachmentContentType",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "AttachmentOriginalFileName",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "CampusLocation",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsEscalated",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ResolutionProofFileName",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ServiceCenterId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "SlaDeadlineUtc",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ArchivedAtUtc",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "Products");
        }
    }
}
