using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMetadataColumnForStandarization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dbs001_user",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_fullname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    user_passwordhash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    user_refreshtoken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    user_refreshtokenexpiresat = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    user_isemailverified = table.Column<bool>(type: "boolean", nullable: false),
                    user_createddate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_updateddate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dbs001_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "dbs001_emailverification",
                columns: table => new
                {
                    emailverification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    emailverification_userid = table.Column<Guid>(type: "uuid", nullable: false),
                    emailverification_token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    emailverification_expiresat = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    emailverification_createddate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dbs001_emailverification", x => x.emailverification_id);
                    table.ForeignKey(
                        name: "fk_dbs001_emailverification_dbs001_user_emailverification_user",
                        column: x => x.emailverification_userid,
                        principalTable: "dbs001_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dbs001_passwordresettoken",
                columns: table => new
                {
                    passwordresettoken_id = table.Column<Guid>(type: "uuid", nullable: false),
                    passwordresettoken_userid = table.Column<Guid>(type: "uuid", nullable: false),
                    passwordresettoken_token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    passwordresettoken_expiresat = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    passwordresettoken_createddate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dbs001_passwordresettoken", x => x.passwordresettoken_id);
                    table.ForeignKey(
                        name: "fk_dbs001_passwordresettoken_dbs001_user_passwordresettoken_us",
                        column: x => x.passwordresettoken_userid,
                        principalTable: "dbs001_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dbs001_todoitem",
                columns: table => new
                {
                    todoitem_id = table.Column<Guid>(type: "uuid", nullable: false),
                    todoitem_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    todoitem_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    todoitem_iscompleted = table.Column<bool>(type: "boolean", nullable: false),
                    todoitem_completedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    todoitem_userid = table.Column<Guid>(type: "uuid", nullable: false),
                    todoitem_isactive = table.Column<bool>(type: "boolean", nullable: false),
                    todoitem_createddate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    todoitem_updateddate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    todoitem_deleteddate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dbs001_todoitem", x => x.todoitem_id);
                    table.ForeignKey(
                        name: "fk_dbs001_todoitem_dbs001_user_todoitem_userid",
                        column: x => x.todoitem_userid,
                        principalTable: "dbs001_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_dbs001_emailverification_emailverification_token",
                table: "dbs001_emailverification",
                column: "emailverification_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dbs001_emailverification_emailverification_userid",
                table: "dbs001_emailverification",
                column: "emailverification_userid");

            migrationBuilder.CreateIndex(
                name: "ix_dbs001_passwordresettoken_passwordresettoken_token",
                table: "dbs001_passwordresettoken",
                column: "passwordresettoken_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dbs001_passwordresettoken_passwordresettoken_userid",
                table: "dbs001_passwordresettoken",
                column: "passwordresettoken_userid");

            migrationBuilder.CreateIndex(
                name: "ix_dbs001_todoitem_todoitem_userid",
                table: "dbs001_todoitem",
                column: "todoitem_userid");

            migrationBuilder.CreateIndex(
                name: "ix_dbs001_user_user_email",
                table: "dbs001_user",
                column: "user_email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dbs001_emailverification");

            migrationBuilder.DropTable(
                name: "dbs001_passwordresettoken");

            migrationBuilder.DropTable(
                name: "dbs001_todoitem");

            migrationBuilder.DropTable(
                name: "dbs001_user");
        }
    }
}
