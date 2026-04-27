using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Güvenior.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringExpenseGenerationTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "RecurringExpenses",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RecurringExpenses",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "LastGeneratedMonth",
                table: "RecurringExpenses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastGeneratedYear",
                table: "RecurringExpenses",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastGeneratedMonth",
                table: "RecurringExpenses");

            migrationBuilder.DropColumn(
                name: "LastGeneratedYear",
                table: "RecurringExpenses");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "RecurringExpenses",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RecurringExpenses",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");
        }
    }
}
