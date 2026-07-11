using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idiomas.Core.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserCredentialTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_credential",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    provider = table.Column<int>(type: "int", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    external_subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_credential", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_credential_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_credential_provider_external_subject",
                table: "user_credential",
                columns: new[] { "provider", "external_subject" },
                unique: true,
                filter: "[external_subject] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_credential_user_id_provider",
                table: "user_credential",
                columns: new[] { "user_id", "provider" },
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO user_credential (id, user_id, provider, password_hash, external_subject, created_at)
                SELECT NEWID(), id, 0, password, NULL, GETUTCDATE()
                FROM [user]
                WHERE password IS NOT NULL
            ");

            migrationBuilder.DropColumn(
                name: "password",
                table: "user");

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "user",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE [user]
                SET password = uc.password_hash
                FROM [user] u
                INNER JOIN user_credential uc ON uc.user_id = u.id
                WHERE uc.provider = 0
            ");

            migrationBuilder.DropIndex(
                name: "IX_user_email",
                table: "user");

            migrationBuilder.DropTable(
                name: "user_credential");
        }
    }
}
