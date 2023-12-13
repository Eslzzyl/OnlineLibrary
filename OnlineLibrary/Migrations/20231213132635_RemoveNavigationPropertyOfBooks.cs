using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLibrary.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNavigationPropertyOfBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowHistories_Books_BookId",
                table: "BorrowHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrentBorrows_Books_BookId",
                table: "CurrentBorrows");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowHistories_Books_BookId",
                table: "BorrowHistories",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrentBorrows_Books_BookId",
                table: "CurrentBorrows",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowHistories_Books_BookId",
                table: "BorrowHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrentBorrows_Books_BookId",
                table: "CurrentBorrows");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowHistories_Books_BookId",
                table: "BorrowHistories",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrentBorrows_Books_BookId",
                table: "CurrentBorrows",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
