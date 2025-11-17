// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Viae.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LocusNameCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AlterDatabase()
                .Annotation(
                    "Npgsql:CollationDefinition:und_nodiac",
                    "und-u-ks-level1,und-u-ks-level1,icu,False"
                )
                .Annotation("Npgsql:Enum:origo", "domesticus,externus")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:origo", "domesticus,externus")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Loca",
                type: "text",
                nullable: false,
                collation: "und_nodiac",
                oldClrType: typeof(string),
                oldType: "text"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Loca",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "und_nodiac"
            );

            migrationBuilder
                .AlterDatabase()
                .Annotation("Npgsql:Enum:origo", "domesticus,externus")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation(
                    "Npgsql:CollationDefinition:und_nodiac",
                    "und-u-ks-level1,und-u-ks-level1,icu,False"
                )
                .OldAnnotation("Npgsql:Enum:origo", "domesticus,externus")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
