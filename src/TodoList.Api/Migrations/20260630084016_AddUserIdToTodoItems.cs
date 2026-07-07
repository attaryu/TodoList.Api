using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTodoItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "todo_items",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateIndex(
                name: "ix_todo_items_user_id",
                table: "todo_items",
                column: "user_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_todo_items_users_user_id",
                table: "todo_items",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_todo_items_users_user_id",
                table: "todo_items"
            );

            migrationBuilder.DropIndex(name: "ix_todo_items_user_id", table: "todo_items");

            migrationBuilder.DropColumn(name: "user_id", table: "todo_items");
        }
    }
}
