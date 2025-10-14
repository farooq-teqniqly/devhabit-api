using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application
{
  /// <inheritdoc />
  public partial class Add_Habits : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.EnsureSchema(name: "devhabit");

      migrationBuilder.CreateTable(
        name: "habits",
        schema: "devhabit",
        columns: table => new
        {
          id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
          created_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
          description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
          end_date = table.Column<DateOnly>(type: "date", nullable: true),
          frequency_times_per_period = table.Column<int>(type: "int", nullable: false),
          frequency_type = table.Column<int>(type: "int", nullable: false),
          status = table.Column<int>(type: "int", nullable: false),
          type = table.Column<int>(type: "int", nullable: false),
          is_archived = table.Column<bool>(type: "bit", nullable: false),
          last_completed_at_utc = table.Column<DateTimeOffset>(
            type: "datetimeoffset",
            nullable: true
          ),
          milestone_current = table.Column<int>(type: "int", nullable: true),
          milestone_target = table.Column<int>(type: "int", nullable: true),
          name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
          target_unit = table.Column<string>(
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false
          ),
          target_value = table.Column<int>(type: "int", nullable: false),
          updated_at_utc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
        },
        constraints: table =>
        {
          table.PrimaryKey("pk_habits", x => x.id);
        }
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(name: "habits", schema: "devhabit");
    }
  }
}
