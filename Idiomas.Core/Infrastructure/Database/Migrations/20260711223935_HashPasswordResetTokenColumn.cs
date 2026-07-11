using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idiomas.Core.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class HashPasswordResetTokenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "token",
                table: "password_reset_token",
                newName: "token_hash");

            migrationBuilder.RenameIndex(
                name: "IX_password_reset_token_token",
                table: "password_reset_token",
                newName: "IX_password_reset_token_token_hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "token_hash",
                table: "password_reset_token",
                newName: "token");

            migrationBuilder.RenameIndex(
                name: "IX_password_reset_token_token_hash",
                table: "password_reset_token",
                newName: "IX_password_reset_token_token");
        }
    }
}
