using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application
{
  /// <inheritdoc />
  public partial class Add_Tags : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "tags",
        schema: "devhabit",
        columns: table => new
        {
          id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
          created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
          description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
          name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
          updated_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
        },
        constraints: table =>
        {
          table.PrimaryKey("pk_tags", x => x.id);
        }
      );

      migrationBuilder.CreateIndex(
        name: "ix_tags_name",
        schema: "devhabit",
        table: "tags",
        column: "name",
        unique: true
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(name: "tags", schema: "devhabit");
    }
  }
}
