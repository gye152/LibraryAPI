using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library_API.Migrations
{
    public partial class FkAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookCopiesId",
                table: "Loans",
                column: "BookCopiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BookCopies_BookCopiesId",
                table: "Loans",
                column: "BookCopiesId",
                principalTable: "BookCopies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BookCopies_BookCopiesId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BookCopiesId",
                table: "Loans");
        }
    }
}
