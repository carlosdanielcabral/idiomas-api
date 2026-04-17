using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idiomas.Core.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultScenarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "scenario",
                columns: new[] { "id", "language", "title", "description", "is_active" },
                values: new object[,]
                {
                    // English
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "English", "At the Restaurant", "Ordering food and drinks at a restaurant", true },
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567891"), "English", "Hotel Check-in", "Checking into a hotel and discussing room preferences", true },
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567892"), "English", "Job Interview", "A professional job interview scenario", true },
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567893"), "English", "Shopping", "Buying clothes and asking about sizes and prices", true },
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567894"), "English", "Doctor's Appointment", "Describing symptoms and discussing health", true },

                    // Spanish
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), "Spanish", "En el Restaurante", "Ordering food at a Spanish restaurant", true },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678902"), "Spanish", "En el Hotel", "Hotel check-in scenario in Spanish", true },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678903"), "Spanish", "Entrevista de Trabajo", "Job interview in Spanish", true },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678904"), "Spanish", "De Compras", "Shopping scenario in Spanish", true },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678905"), "Spanish", "En el Médico", "Doctor's appointment in Spanish", true },

                    // French
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), "French", "Au Restaurant", "Ordering food at a French restaurant", true },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789013"), "French", "À l'Hôtel", "Hotel check-in in French", true },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789014"), "French", "Entretien d'Embauche", "Job interview in French", true },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789015"), "French", "Shopping", "Shopping scenario in French", true },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789016"), "French", "Chez le Médecin", "Doctor's appointment in French", true },

                    // German
                    { new Guid("d4e5f6a7-b8c9-0123-def1-234567890123"), "German", "Im Restaurant", "Ordering food at a German restaurant", true },
                    { new Guid("d4e5f6a7-b8c9-0123-def1-234567890124"), "German", "Im Hotel", "Hotel check-in in German", true },
                    { new Guid("d4e5f6a7-b8c9-0123-def1-234567890125"), "German", "Vorstellungsgespräch", "Job interview in German", true },
                    { new Guid("d4e5f6a7-b8c9-0123-def1-234567890126"), "German", "Einkaufen", "Shopping scenario in German", true },
                    { new Guid("d4e5f6a7-b8c9-0123-def1-234567890127"), "German", "Beim Arzt", "Doctor's appointment in German", true },

                    // Italian
                    { new Guid("e5f6a7b8-c9d0-1234-ef12-345678901234"), "Italian", "Al Ristorante", "Ordering food at an Italian restaurant", true },
                    { new Guid("e5f6a7b8-c9d0-1234-ef12-345678901235"), "Italian", "In Hotel", "Hotel check-in in Italian", true },
                    { new Guid("e5f6a7b8-c9d0-1234-ef12-345678901236"), "Italian", "Colloquio di Lavoro", "Job interview in Italian", true },
                    { new Guid("e5f6a7b8-c9d0-1234-ef12-345678901237"), "Italian", "Shopping", "Shopping scenario in Italian", true },
                    { new Guid("e5f6a7b8-c9d0-1234-ef12-345678901238"), "Italian", "Dal Dottore", "Doctor's appointment in Italian", true },

                    // Portuguese
                    { new Guid("f6a7b8c9-d0e1-2345-f123-456789012345"), "Portuguese", "No Restaurante", "Ordering food at a Portuguese restaurant", true },
                    { new Guid("f6a7b8c9-d0e1-2345-f123-456789012346"), "Portuguese", "No Hotel", "Hotel check-in in Portuguese", true },
                    { new Guid("f6a7b8c9-d0e1-2345-f123-456789012347"), "Portuguese", "Entrevista de Emprego", "Job interview in Portuguese", true },
                    { new Guid("f6a7b8c9-d0e1-2345-f123-456789012348"), "Portuguese", "Fazendo Compras", "Shopping scenario in Portuguese", true },
                    { new Guid("f6a7b8c9-d0e1-2345-f123-456789012349"), "Portuguese", "No Médico", "Doctor's appointment in Portuguese", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "scenario",
                keyColumn: "id",
                keyValues: new object[]
                {
                    new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                    new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567891"),
                    new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567892"),
                    new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567893"),
                    new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567894"),
                    new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                    new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678902"),
                    new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678903"),
                    new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678904"),
                    new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678905"),
                    new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"),
                    new Guid("c3d4e5f6-a7b8-9012-cdef-123456789013"),
                    new Guid("c3d4e5f6-a7b8-9012-cdef-123456789014"),
                    new Guid("c3d4e5f6-a7b8-9012-cdef-123456789015"),
                    new Guid("c3d4e5f6-a7b8-9012-cdef-123456789016"),
                    new Guid("d4e5f6a7-b8c9-0123-def1-234567890123"),
                    new Guid("d4e5f6a7-b8c9-0123-def1-234567890124"),
                    new Guid("d4e5f6a7-b8c9-0123-def1-234567890125"),
                    new Guid("d4e5f6a7-b8c9-0123-def1-234567890126"),
                    new Guid("d4e5f6a7-b8c9-0123-def1-234567890127"),
                    new Guid("e5f6a7b8-c9d0-1234-ef12-345678901234"),
                    new Guid("e5f6a7b8-c9d0-1234-ef12-345678901235"),
                    new Guid("e5f6a7b8-c9d0-1234-ef12-345678901236"),
                    new Guid("e5f6a7b8-c9d0-1234-ef12-345678901237"),
                    new Guid("e5f6a7b8-c9d0-1234-ef12-345678901238"),
                    new Guid("f6a7b8c9-d0e1-2345-f123-456789012345"),
                    new Guid("f6a7b8c9-d0e1-2345-f123-456789012346"),
                    new Guid("f6a7b8c9-d0e1-2345-f123-456789012347"),
                    new Guid("f6a7b8c9-d0e1-2345-f123-456789012348"),
                    new Guid("f6a7b8c9-d0e1-2345-f123-456789012349")
                });
        }
    }
}
