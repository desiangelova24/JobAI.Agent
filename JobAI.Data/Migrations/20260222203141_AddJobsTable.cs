using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobAI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RemoteJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Technologies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LanguageLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalaryEUR = table.Column<double>(type: "float", nullable: false),
                    Advice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchScore = table.Column<int>(type: "int", nullable: false),
                    CompanyOrigin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSaved = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoteJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemoteJobs_ExternalId",
                table: "RemoteJobs",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemoteJobs");
        }
    }
}
