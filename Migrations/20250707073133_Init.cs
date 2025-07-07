using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiataBot.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guild_configurations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guild_configurations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "owners",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    default_car_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_owners", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "copypastas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_copypastas", x => x.id);
                    table.ForeignKey(
                        name: "fk_copypastas_guild_configurations_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guild_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "timed_role_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    granted_role_ids = table.Column<long[]>(type: "bigint[]", nullable: false),
                    revoked_role_ids = table.Column<long[]>(type: "bigint[]", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_timed_role_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_timed_role_entries_guild_configurations_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guild_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<long>(type: "bigint", nullable: false),
                    owned_since = table.Column<DateOnly>(type: "date", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    make = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    pet_name = table.Column<string>(type: "text", nullable: true),
                    blurb = table.Column<string>(type: "text", nullable: true),
                    vin = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cars", x => x.id);
                    table.ForeignKey(
                        name: "fk_cars_owners_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owners",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "media",
                columns: table => new
                {
                    object_key = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media", x => x.object_key);
                    table.ForeignKey(
                        name: "fk_media_cars_car_id",
                        column: x => x.car_id,
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "metadata",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_metadata", x => new { x.car_id, x.name });
                    table.ForeignKey(
                        name: "fk_metadata_cars_car_id",
                        column: x => x.car_id,
                        principalTable: "cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cars_owner_id",
                table: "cars",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_copypastas_guild_id",
                table: "copypastas",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_copypastas_user_id",
                table: "copypastas",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_car_id",
                table: "media",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "ix_timed_role_entries_guild_id",
                table: "timed_role_entries",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_timed_role_entries_user_id",
                table: "timed_role_entries",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "copypastas");

            migrationBuilder.DropTable(
                name: "media");

            migrationBuilder.DropTable(
                name: "metadata");

            migrationBuilder.DropTable(
                name: "timed_role_entries");

            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.DropTable(
                name: "guild_configurations");

            migrationBuilder.DropTable(
                name: "owners");
        }
    }
}
