using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBasketItemIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BasketItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BasketItems");
        }
    }
}
