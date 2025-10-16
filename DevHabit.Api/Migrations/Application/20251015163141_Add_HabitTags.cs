using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application
{
  /// <inheritdoc />
  public partial class Add_HabitTags : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "habit_tags",
        schema: "devhabit",
        columns: table => new
        {
          habit_id = table.Column<string>(type: "nvarchar(100)", nullable: false),
          tag_id = table.Column<string>(type: "nvarchar(100)", nullable: false),
          created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("pk_habit_tags", x => new { x.habit_id, x.tag_id });
          table.ForeignKey(
            name: "fk_habit_tags_habits_habit_id",
            column: x => x.habit_id,
            principalSchema: "devhabit",
            principalTable: "habits",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade
          );
          table.ForeignKey(
            name: "fk_habit_tags_tags_tag_id",
            column: x => x.tag_id,
            principalSchema: "devhabit",
            principalTable: "tags",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade
          );
        }
      );

      migrationBuilder.CreateIndex(
        name: "ix_habit_tags_tag_id",
        schema: "devhabit",
        table: "habit_tags",
        column: "tag_id"
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(name: "habit_tags", schema: "devhabit");
    }
  }
}
