using Microsoft.EntityFrameworkCore.Migrations;

namespace Tool_Facebook.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblViaPage",
                columns: table => new
                {
                    C_UID = table.Column<string>(nullable: false),
                    C_Password = table.Column<string>(nullable: true),
                    C_Email = table.Column<string>(nullable: true),
                    C_PassEmail = table.Column<string>(nullable: true),
                    C_2FA = table.Column<string>(nullable: true),
                    C_Status = table.Column<string>(nullable: true),
                    C_Folder = table.Column<string>(nullable: true),
                    C_Cookie = table.Column<string>(nullable: true),
                    C_Token = table.Column<string>(nullable: true),
                    C_Proxy = table.Column<string>(nullable: true),
                    C_UserAgent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblViaPage", x => x.C_UID);
                });

            migrationBuilder.CreateTable(
                name: "tblPages",
                columns: table => new
                {
                    C_IDPage = table.Column<string>(nullable: false),
                    C_NamePage = table.Column<string>(nullable: true),
                    C_Follower = table.Column<string>(nullable: true),
                    C_StatusPage = table.Column<string>(nullable: true),
                    C_UID = table.Column<string>(nullable: true),
                    tblViaC_UID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPages", x => x.C_IDPage);
                    table.ForeignKey(
                        name: "FK_tblPages_tblViaPage_tblViaC_UID",
                        column: x => x.tblViaC_UID,
                        principalTable: "tblViaPage",
                        principalColumn: "C_UID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblPages_tblViaC_UID",
                table: "tblPages",
                column: "tblViaC_UID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblPages");

            migrationBuilder.DropTable(
                name: "tblViaPage");
        }
    }
}
