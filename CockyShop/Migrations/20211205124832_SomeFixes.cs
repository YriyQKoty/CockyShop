using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CockyShop.Migrations
{
    public partial class SomeFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersDetails_Orders_OrderId",
                table: "OrdersDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrdersDetails_OrderId",
                table: "OrdersDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "Stocks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "ProductStocks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "OrderStatuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "OrdersDetails",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "Orders",
                newName: "OrderDetailsId");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "OrderedProducts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "Cities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "AspNetUsers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "AspNetUserClaims",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "AspNetRoles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GeneralProductId",
                table: "AspNetRoleClaims",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "OrderDetailsId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "OrderDetailsId2",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDetailsId",
                table: "Orders",
                column: "OrderDetailsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDetailsId2",
                table: "Orders",
                column: "OrderDetailsId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrdersDetails_OrderDetailsId",
                table: "Orders",
                column: "OrderDetailsId",
                principalTable: "OrdersDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrdersDetails_OrderDetailsId2",
                table: "Orders",
                column: "OrderDetailsId2",
                principalTable: "OrdersDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrdersDetails_OrderDetailsId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrdersDetails_OrderDetailsId2",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderDetailsId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderDetailsId2",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderDetailsId2",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Stocks",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProductStocks",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Products",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderStatuses",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrdersDetails",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "OrderDetailsId",
                table: "Orders",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderedProducts",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cities",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetUsers",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetUserClaims",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetRoles",
                newName: "GeneralProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetRoleClaims",
                newName: "GeneralProductId");

            migrationBuilder.AlterColumn<int>(
                name: "GeneralProductId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "GeneralProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersDetails_OrderId",
                table: "OrdersDetails",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersDetails_Orders_OrderId",
                table: "OrdersDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "GeneralProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
