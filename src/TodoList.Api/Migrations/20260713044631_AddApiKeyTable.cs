using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddApiKeyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dbs001_apikeys",
                columns: table => new
                {
                    apikey_id = table.Column<Guid>(type: "uuid", nullable: false),
                    apikey_keyhash = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    apikey_prefix = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    apikey_label = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    apikey_userid = table.Column<Guid>(type: "uuid", nullable: false),
                    apikey_createddate = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    apikey_updateddate = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    apikey_isactive = table.Column<bool>(type: "boolean", nullable: false),
                    apikey_deleteddate = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dbs001_apikeys", x => x.apikey_id);
                    table.ForeignKey(
                        name: "fk_dbs001_apikeys_dbs001_user_apikey_userid",
                        column: x => x.apikey_userid,
                        principalTable: "dbs001_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_dbs001_apikeys_apikey_keyhash",
                table: "dbs001_apikeys",
                column: "apikey_keyhash",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_dbs001_apikeys_apikey_userid",
                table: "dbs001_apikeys",
                column: "apikey_userid"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "dbs001_apikeys");
        }
    }
}
