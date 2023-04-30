using Microsoft.EntityFrameworkCore.Migrations;

namespace Site.Migrations
{
    public partial class MAddForeignKeyWebsiteLanguagePages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WebsiteLanguagesId",
                table: "Websites",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Websites_WebsiteLanguagesId",
                table: "Websites",
                column: "WebsiteLanguagesId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteLanguages_LanguageId",
                table: "WebsiteLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_WebsiteLanguageId",
                table: "Pages",
                column: "WebsiteLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_WebsiteLanguages",
                table: "Pages",
                column: "WebsiteLanguageId",
                principalTable: "WebsiteLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebsiteLanguages_Languages_LanguageId",
                table: "WebsiteLanguages",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Websites_WebsiteLanguages_WebsiteLanguagesId",
                table: "Websites",
                column: "WebsiteLanguagesId",
                principalTable: "WebsiteLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_WebsiteLanguages",
                table: "Pages");

            migrationBuilder.DropForeignKey(
                name: "FK_WebsiteLanguages_Languages_LanguageId",
                table: "WebsiteLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_Websites_WebsiteLanguages_WebsiteLanguagesId",
                table: "Websites");

            migrationBuilder.DropIndex(
                name: "IX_Websites_WebsiteLanguagesId",
                table: "Websites");

            migrationBuilder.DropIndex(
                name: "IX_WebsiteLanguages_LanguageId",
                table: "WebsiteLanguages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_WebsiteLanguageId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "WebsiteLanguagesId",
                table: "Websites");
        }
    }
}
