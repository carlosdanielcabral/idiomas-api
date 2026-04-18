using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idiomas.Core.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scenario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scenario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "conversation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    mode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    scenario_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversation", x => x.id);
                    table.ForeignKey(
                        name: "FK_conversation_scenario_scenario_id",
                        column: x => x.scenario_id,
                        principalTable: "scenario",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_conversation_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.id);
                    table.ForeignKey(
                        name: "FK_message_conversation_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "conversation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "correction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    message_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    original_fragment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    suggested_fragment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    explanation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_correction", x => x.id);
                    table.ForeignKey(
                        name: "FK_correction_message_message_id",
                        column: x => x.message_id,
                        principalTable: "message",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_conversation_is_active",
                table: "conversation",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_conversation_scenario_id",
                table: "conversation",
                column: "scenario_id");

            migrationBuilder.CreateIndex(
                name: "IX_conversation_user_id",
                table: "conversation",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_correction_message_id",
                table: "correction",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_message_conversation_id",
                table: "message",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "IX_message_conversation_id_created_at",
                table: "message",
                columns: new[] { "conversation_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_scenario_language",
                table: "scenario",
                column: "language");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "correction");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "conversation");

            migrationBuilder.DropTable(
                name: "scenario");
        }
    }
}
