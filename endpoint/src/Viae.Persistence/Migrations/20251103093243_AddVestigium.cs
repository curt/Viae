// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore.Migrations;
using Viae.Domain.Models;

#nullable disable

namespace Viae.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVestigium : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vestigia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    PersonaId = table.Column<string>(type: "char(11)", nullable: false),
                    LocusId = table.Column<string>(type: "char(11)", nullable: false),
                    HappenedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    Origo = table.Column<Origo>(type: "origo", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    PublishedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    TombstonedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vestigia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vestigia_Loca_LocusId",
                        column: x => x.LocusId,
                        principalTable: "Loca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Vestigia_Personae_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personae",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Vestigia_LocusId",
                table: "Vestigia",
                column: "LocusId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Vestigia_PersonaId",
                table: "Vestigia",
                column: "PersonaId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Vestigia");
        }
    }
}
