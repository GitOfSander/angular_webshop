using Microsoft.EntityFrameworkCore.Migrations;

namespace Site.Migrations
{
    public partial class M131 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefInvoiceNumber",
                table: "Orders",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefInvoiceNumber",
                table: "Orders");
        }
    }
}
