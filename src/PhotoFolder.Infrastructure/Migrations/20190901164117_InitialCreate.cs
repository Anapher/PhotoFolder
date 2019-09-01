using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoFolder.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileOperations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    TargetFilename = table.Column<string>(nullable: false),
                    TargetHash = table.Column<string>(nullable: false),
                    SourceFilename = table.Column<string>(nullable: true),
                    SourceHash = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndexedFiles",
                columns: table => new
                {
                    Hash = table.Column<string>(nullable: false),
                    Length = table.Column<long>(nullable: false),
                    FileCreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    PhotoBitmapHash = table.Column<string>(nullable: true),
                    PhotoWidth = table.Column<int>(nullable: true),
                    PhotoHeight = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexedFiles", x => x.Hash);
                });

            migrationBuilder.CreateTable(
                name: "FileLocation",
                columns: table => new
                {
                    RelativeFilename = table.Column<string>(nullable: false),
                    Hash = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    IndexedFileHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileLocation", x => x.RelativeFilename);
                    table.ForeignKey(
                        name: "FK_FileLocation_IndexedFiles_IndexedFileHash",
                        column: x => x.IndexedFileHash,
                        principalTable: "IndexedFiles",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileLocation_IndexedFileHash",
                table: "FileLocation",
                column: "IndexedFileHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileLocation");

            migrationBuilder.DropTable(
                name: "FileOperations");

            migrationBuilder.DropTable(
                name: "IndexedFiles");
        }
    }
}
