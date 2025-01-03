using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExcalibPayments.Orders.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MerchantId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    WebSite = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MerchantId",
                table: "Orders",
                column: "MerchantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Merchants_MerchantId",
                table: "Orders",
                column: "MerchantId",
                principalTable: "Merchants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Merchants_MerchantId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropIndex(
                name: "IX_Orders_MerchantId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MerchantId",
                table: "Orders");
        }
    }
}
