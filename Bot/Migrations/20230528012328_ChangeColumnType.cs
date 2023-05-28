using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Migrations
{
    public partial class ChangeColumnType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Travels_TravelId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "TravelId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Travels_TravelId",
                table: "Users",
                column: "TravelId",
                principalTable: "Travels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Travels_TravelId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "TravelId",
                table: "Users",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Travels_TravelId",
                table: "Users",
                column: "TravelId",
                principalTable: "Travels",
                principalColumn: "Id");
        }
    }
}
