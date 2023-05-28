using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Migrations
{
    public partial class AddDefaultValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Travels_Users_UserId",
                table: "Travels");

            migrationBuilder.DropIndex(
                name: "IX_Travels_UserId",
                table: "Travels");

            migrationBuilder.DropColumn(
                name: "People",
                table: "Travels");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Travels");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "PictureFileId",
                table: "Travels",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Travels",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Travels",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "TravelUser",
                columns: table => new
                {
                    PeopleId = table.Column<int>(type: "INTEGER", nullable: false),
                    TravelsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelUser", x => new { x.PeopleId, x.TravelsId });
                    table.ForeignKey(
                        name: "FK_TravelUser_Travels_TravelsId",
                        column: x => x.TravelsId,
                        principalTable: "Travels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelUser_Users_PeopleId",
                        column: x => x.PeopleId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravelUser_TravelsId",
                table: "TravelUser",
                column: "TravelsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelUser");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PictureFileId",
                table: "Travels",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Travels",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Travels",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "People",
                table: "Travels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Travels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Travels_UserId",
                table: "Travels",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Travels_Users_UserId",
                table: "Travels",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
