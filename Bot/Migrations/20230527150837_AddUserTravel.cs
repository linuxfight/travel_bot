using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Migrations
{
    public partial class AddUserTravel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelUser");

            migrationBuilder.AddColumn<int>(
                name: "TravelId",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TravelId",
                table: "Users",
                column: "TravelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Travels_TravelId",
                table: "Users",
                column: "TravelId",
                principalTable: "Travels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Travels_TravelId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TravelId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TravelId",
                table: "Users");

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
    }
}
