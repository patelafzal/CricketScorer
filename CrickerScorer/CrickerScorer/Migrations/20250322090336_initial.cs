using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CrickerScorer.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BallActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeliveryNumber = table.Column<int>(type: "integer", nullable: false),
                    LegalBallNumber = table.Column<int>(type: "integer", nullable: true),
                    Runs = table.Column<int>(type: "integer", nullable: false),
                    ExtraRuns = table.Column<int>(type: "integer", nullable: false),
                    OverthrowRuns = table.Column<int>(type: "integer", nullable: false),
                    BallType = table.Column<int>(type: "integer", nullable: false),
                    Wicket = table.Column<int>(type: "integer", nullable: false),
                    Commentary = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShortCode = table.Column<string>(type: "text", nullable: true),
                    ColorClass = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BallActions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BallActions");
        }
    }
}
