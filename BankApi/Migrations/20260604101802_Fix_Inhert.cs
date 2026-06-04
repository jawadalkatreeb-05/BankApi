using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApi.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Inhert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Accounts",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WithdrawalLimit",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "WithdrawalLimit",
                table: "Accounts");
        }
    }
}
