// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Viae.Domain.Models;

#nullable disable

namespace Viae.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AlterDatabase()
                .Annotation("Npgsql:Enum:origo", "domesticus,externus")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Abmissiones",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    Actor = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    ReceivedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    PongSent = table.Column<bool>(type: "boolean", nullable: false),
                    PongSentAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abmissiones", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Abreflexiones",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    Actor = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    PingUri = table.Column<string>(type: "text", nullable: false),
                    ReceivedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abreflexiones", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Admissiones",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    Actor = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    SentAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    PongReceived = table.Column<bool>(type: "boolean", nullable: false),
                    PongReceivedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admissiones", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Adreflexiones",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    Actor = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    PingUri = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    SentAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adreflexiones", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Loca",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Coordinates = table.Column<Point>(
                        type: "geography(Point, 4326)",
                        nullable: false
                    ),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loca", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Personae",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    Origo = table.Column<Origo>(type: "origo", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CognitoUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    Uri = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    PublicKey = table.Column<string>(type: "text", nullable: true),
                    PrivateKey = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personae", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Themata",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themata", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Folios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(11)", nullable: false),
                    Origo = table.Column<Origo>(type: "origo", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PersonaId = table.Column<string>(type: "char(11)", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    PublishedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    ImageUrls = table.Column<List<string>>(type: "text[]", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folios_Personae_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personae",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "LocusThema",
                columns: table => new
                {
                    LocaId = table.Column<string>(type: "char(11)", nullable: false),
                    ThemataId = table.Column<string>(type: "char(11)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocusThema", x => new { x.LocaId, x.ThemataId });
                    table.ForeignKey(
                        name: "FK_LocusThema_Loca_LocaId",
                        column: x => x.LocaId,
                        principalTable: "Loca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_LocusThema_Themata_ThemataId",
                        column: x => x.ThemataId,
                        principalTable: "Themata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "FoliumLocus",
                columns: table => new
                {
                    FoliosId = table.Column<string>(type: "char(11)", nullable: false),
                    LocaId = table.Column<string>(type: "char(11)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoliumLocus", x => new { x.FoliosId, x.LocaId });
                    table.ForeignKey(
                        name: "FK_FoliumLocus_Folios_FoliosId",
                        column: x => x.FoliosId,
                        principalTable: "Folios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_FoliumLocus_Loca_LocaId",
                        column: x => x.LocaId,
                        principalTable: "Loca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "FoliumThema",
                columns: table => new
                {
                    FoliosId = table.Column<string>(type: "char(11)", nullable: false),
                    ThemataId = table.Column<string>(type: "char(11)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoliumThema", x => new { x.FoliosId, x.ThemataId });
                    table.ForeignKey(
                        name: "FK_FoliumThema_Folios_FoliosId",
                        column: x => x.FoliosId,
                        principalTable: "Folios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_FoliumThema_Themata_ThemataId",
                        column: x => x.ThemataId,
                        principalTable: "Themata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Folios_IsDraft",
                table: "Folios",
                column: "IsDraft"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Folios_PersonaId",
                table: "Folios",
                column: "PersonaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Folios_PublishedAt",
                table: "Folios",
                column: "PublishedAt"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FoliumLocus_LocaId",
                table: "FoliumLocus",
                column: "LocaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FoliumThema_ThemataId",
                table: "FoliumThema",
                column: "ThemataId"
            );

            migrationBuilder
                .CreateIndex(name: "IX_Loca_Coordinates", table: "Loca", column: "Coordinates")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(name: "IX_Loca_Name", table: "Loca", column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LocusThema_ThemataId",
                table: "LocusThema",
                column: "ThemataId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Personae_CognitoUserId",
                table: "Personae",
                column: "CognitoUserId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Personae_Email",
                table: "Personae",
                column: "Email"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Personae_Uri",
                table: "Personae",
                column: "Uri",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Personae_Username",
                table: "Personae",
                column: "Username",
                unique: true
            );

            migrationBuilder.CreateIndex(name: "IX_Themata_Name", table: "Themata", column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Themata_Slug",
                table: "Themata",
                column: "Slug",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Abmissiones");

            migrationBuilder.DropTable(name: "Abreflexiones");

            migrationBuilder.DropTable(name: "Admissiones");

            migrationBuilder.DropTable(name: "Adreflexiones");

            migrationBuilder.DropTable(name: "FoliumLocus");

            migrationBuilder.DropTable(name: "FoliumThema");

            migrationBuilder.DropTable(name: "LocusThema");

            migrationBuilder.DropTable(name: "Folios");

            migrationBuilder.DropTable(name: "Loca");

            migrationBuilder.DropTable(name: "Themata");

            migrationBuilder.DropTable(name: "Personae");
        }
    }
}
