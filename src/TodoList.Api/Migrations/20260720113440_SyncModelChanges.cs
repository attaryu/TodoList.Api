using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dbs001_apikeys_dbs001_user_apikey_userid",
                table: "dbs001_apikeys");

            migrationBuilder.DropForeignKey(
                name: "fk_dbs001_emailverification_dbs001_user_emailverification_user",
                table: "dbs001_emailverification");

            migrationBuilder.DropForeignKey(
                name: "fk_dbs001_passwordresettoken_dbs001_user_passwordresettoken_us",
                table: "dbs001_passwordresettoken");

            migrationBuilder.DropForeignKey(
                name: "fk_dbs001_todoitem_dbs001_user_todoitem_userid",
                table: "dbs001_todoitem");

            migrationBuilder.DropPrimaryKey(
                name: "pk_dbs001_user",
                table: "dbs001_user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_dbs001_todoitem",
                table: "dbs001_todoitem");

            migrationBuilder.DropPrimaryKey(
                name: "pk_dbs001_passwordresettoken",
                table: "dbs001_passwordresettoken");

            migrationBuilder.DropPrimaryKey(
                name: "pk_dbs001_emailverification",
                table: "dbs001_emailverification");

            migrationBuilder.DropPrimaryKey(
                name: "pk_dbs001_apikeys",
                table: "dbs001_apikeys");

            migrationBuilder.RenameIndex(
                name: "ix_dbs001_user_user_email",
                table: "dbs001_user",
                newName: "IX_dbs001_user_user_email");

            migrationBuilder.RenameIndex(
                name: "ix_dbs001_todoitem_todoitem_userid",
                table: "dbs001_todoitem",
                newName: "IX_dbs001_todoitem_todoitem_userid");

            migrationBuilder.RenameIndex(
                name: "ix_dbs001_passwordresettoken_passwordresettoken_userid",
                table: "dbs001_passwordresettoken",
                newName: "IX_dbs001_passwordresettoken_passwordresettoken_userid");

            migrationBuilder.RenameIndex(
                name: "ix_dbs001_passwordresettoken_passwordresettoken_token",
                table: "dbs001_passwordresettoken",
                newName: "IX_dbs001_passwordresettoken_passwordresettoken_token");

            migrationBuilder.RenameIndex(
                name: "ix_dbs001_emailverification_emailverification_userid",
                table: "dbs001_emailverification",
                newName: "IX_dbs001_emailverification_emailverification_userid");

            migrationBuilder.RenameIndex(
                name: "ix_dbs001_emailverification_emailverification_token",
                table: "dbs001_emailverification",
                newName: "IX_dbs001_emailverification_emailverification_token");

            migrationBuilder.RenameIndex(
                name: "ix_dbs001_apikeys_apikey_userid",
                table: "dbs001_apikeys",
                newName: "IX_dbs001_apikeys_apikey_userid");

            migrationBuilder.RenameIndex(
                name: "ix_dbs001_apikeys_apikey_keyhash",
                table: "dbs001_apikeys",
                newName: "IX_dbs001_apikeys_apikey_keyhash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbs001_user",
                table: "dbs001_user",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbs001_todoitem",
                table: "dbs001_todoitem",
                column: "todoitem_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbs001_passwordresettoken",
                table: "dbs001_passwordresettoken",
                column: "passwordresettoken_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbs001_emailverification",
                table: "dbs001_emailverification",
                column: "emailverification_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbs001_apikeys",
                table: "dbs001_apikeys",
                column: "apikey_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dbs001_apikeys_dbs001_user_apikey_userid",
                table: "dbs001_apikeys",
                column: "apikey_userid",
                principalTable: "dbs001_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbs001_emailverification_dbs001_user_emailverification_user~",
                table: "dbs001_emailverification",
                column: "emailverification_userid",
                principalTable: "dbs001_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbs001_passwordresettoken_dbs001_user_passwordresettoken_us~",
                table: "dbs001_passwordresettoken",
                column: "passwordresettoken_userid",
                principalTable: "dbs001_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbs001_todoitem_dbs001_user_todoitem_userid",
                table: "dbs001_todoitem",
                column: "todoitem_userid",
                principalTable: "dbs001_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbs001_apikeys_dbs001_user_apikey_userid",
                table: "dbs001_apikeys");

            migrationBuilder.DropForeignKey(
                name: "FK_dbs001_emailverification_dbs001_user_emailverification_user~",
                table: "dbs001_emailverification");

            migrationBuilder.DropForeignKey(
                name: "FK_dbs001_passwordresettoken_dbs001_user_passwordresettoken_us~",
                table: "dbs001_passwordresettoken");

            migrationBuilder.DropForeignKey(
                name: "FK_dbs001_todoitem_dbs001_user_todoitem_userid",
                table: "dbs001_todoitem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbs001_user",
                table: "dbs001_user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbs001_todoitem",
                table: "dbs001_todoitem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbs001_passwordresettoken",
                table: "dbs001_passwordresettoken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbs001_emailverification",
                table: "dbs001_emailverification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbs001_apikeys",
                table: "dbs001_apikeys");

            migrationBuilder.RenameIndex(
                name: "IX_dbs001_user_user_email",
                table: "dbs001_user",
                newName: "ix_dbs001_user_user_email");

            migrationBuilder.RenameIndex(
                name: "IX_dbs001_todoitem_todoitem_userid",
                table: "dbs001_todoitem",
                newName: "ix_dbs001_todoitem_todoitem_userid");

            migrationBuilder.RenameIndex(
                name: "IX_dbs001_passwordresettoken_passwordresettoken_userid",
                table: "dbs001_passwordresettoken",
                newName: "ix_dbs001_passwordresettoken_passwordresettoken_userid");

            migrationBuilder.RenameIndex(
                name: "IX_dbs001_passwordresettoken_passwordresettoken_token",
                table: "dbs001_passwordresettoken",
                newName: "ix_dbs001_passwordresettoken_passwordresettoken_token");

            migrationBuilder.RenameIndex(
                name: "IX_dbs001_emailverification_emailverification_userid",
                table: "dbs001_emailverification",
                newName: "ix_dbs001_emailverification_emailverification_userid");

            migrationBuilder.RenameIndex(
                name: "IX_dbs001_emailverification_emailverification_token",
                table: "dbs001_emailverification",
                newName: "ix_dbs001_emailverification_emailverification_token");

            migrationBuilder.RenameIndex(
                name: "IX_dbs001_apikeys_apikey_userid",
                table: "dbs001_apikeys",
                newName: "ix_dbs001_apikeys_apikey_userid");

            migrationBuilder.RenameIndex(
                name: "IX_dbs001_apikeys_apikey_keyhash",
                table: "dbs001_apikeys",
                newName: "ix_dbs001_apikeys_apikey_keyhash");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dbs001_user",
                table: "dbs001_user",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dbs001_todoitem",
                table: "dbs001_todoitem",
                column: "todoitem_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dbs001_passwordresettoken",
                table: "dbs001_passwordresettoken",
                column: "passwordresettoken_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dbs001_emailverification",
                table: "dbs001_emailverification",
                column: "emailverification_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_dbs001_apikeys",
                table: "dbs001_apikeys",
                column: "apikey_id");

            migrationBuilder.AddForeignKey(
                name: "fk_dbs001_apikeys_dbs001_user_apikey_userid",
                table: "dbs001_apikeys",
                column: "apikey_userid",
                principalTable: "dbs001_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_dbs001_emailverification_dbs001_user_emailverification_user",
                table: "dbs001_emailverification",
                column: "emailverification_userid",
                principalTable: "dbs001_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_dbs001_passwordresettoken_dbs001_user_passwordresettoken_us",
                table: "dbs001_passwordresettoken",
                column: "passwordresettoken_userid",
                principalTable: "dbs001_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_dbs001_todoitem_dbs001_user_todoitem_userid",
                table: "dbs001_todoitem",
                column: "todoitem_userid",
                principalTable: "dbs001_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
