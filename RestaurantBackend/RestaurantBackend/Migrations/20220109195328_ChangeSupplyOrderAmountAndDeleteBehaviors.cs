using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantBackend.Migrations
{
    public partial class ChangeSupplyOrderAmountAndDeleteBehaviors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DishIngredients_Dishes_DishId",
                table: "DishIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_DishIngredients_Ingredients_IngredientId",
                table: "DishIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDishes_CustomerOrders_OrderId",
                table: "OrderDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDishes_Dishes_DishId",
                table: "OrderDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderIngredients_Ingredients_IngredientId",
                table: "OrderIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderIngredients_SupplierOrders_SupplierOrderId",
                table: "OrderIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Tables_TableId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantIngredients_Ingredients_IngredientId",
                table: "RestaurantIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantIngredients_Restaurants_RestaurantId",
                table: "RestaurantIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_OwnerId",
                table: "Restaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierIngredients_Ingredients_IngredientId",
                table: "SupplierIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierIngredients_Suppliers_SupplierId",
                table: "SupplierIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_Restaurants_RestaurantId",
                table: "SupplierOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_Suppliers_SupplierId",
                table: "SupplierOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Restaurants_RestaurantId",
                table: "Tables");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Restaurants_RestaurantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "SupplierOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DishIngredients_Dishes_DishId",
                table: "DishIngredients",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DishIngredients_Ingredients_IngredientId",
                table: "DishIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDishes_CustomerOrders_OrderId",
                table: "OrderDishes",
                column: "OrderId",
                principalTable: "CustomerOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDishes_Dishes_DishId",
                table: "OrderDishes",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderIngredients_Ingredients_IngredientId",
                table: "OrderIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderIngredients_SupplierOrders_SupplierOrderId",
                table: "OrderIngredients",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Tables_TableId",
                table: "Reservations",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantIngredients_Ingredients_IngredientId",
                table: "RestaurantIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantIngredients_Restaurants_RestaurantId",
                table: "RestaurantIngredients",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_OwnerId",
                table: "Restaurants",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierIngredients_Ingredients_IngredientId",
                table: "SupplierIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierIngredients_Suppliers_SupplierId",
                table: "SupplierIngredients",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrders_Restaurants_RestaurantId",
                table: "SupplierOrders",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrders_Suppliers_SupplierId",
                table: "SupplierOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Restaurants_RestaurantId",
                table: "Tables",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Restaurants_RestaurantId",
                table: "Users",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DishIngredients_Dishes_DishId",
                table: "DishIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_DishIngredients_Ingredients_IngredientId",
                table: "DishIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDishes_CustomerOrders_OrderId",
                table: "OrderDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDishes_Dishes_DishId",
                table: "OrderDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderIngredients_Ingredients_IngredientId",
                table: "OrderIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderIngredients_SupplierOrders_SupplierOrderId",
                table: "OrderIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Tables_TableId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantIngredients_Ingredients_IngredientId",
                table: "RestaurantIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantIngredients_Restaurants_RestaurantId",
                table: "RestaurantIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_OwnerId",
                table: "Restaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierIngredients_Ingredients_IngredientId",
                table: "SupplierIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierIngredients_Suppliers_SupplierId",
                table: "SupplierIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_Restaurants_RestaurantId",
                table: "SupplierOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_Suppliers_SupplierId",
                table: "SupplierOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Restaurants_RestaurantId",
                table: "Tables");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Restaurants_RestaurantId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "SupplierOrders",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Menus_MenuId",
                table: "Dishes",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DishIngredients_Dishes_DishId",
                table: "DishIngredients",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DishIngredients_Ingredients_IngredientId",
                table: "DishIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDishes_CustomerOrders_OrderId",
                table: "OrderDishes",
                column: "OrderId",
                principalTable: "CustomerOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDishes_Dishes_DishId",
                table: "OrderDishes",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderIngredients_Ingredients_IngredientId",
                table: "OrderIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderIngredients_SupplierOrders_SupplierOrderId",
                table: "OrderIngredients",
                column: "SupplierOrderId",
                principalTable: "SupplierOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Tables_TableId",
                table: "Reservations",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantIngredients_Ingredients_IngredientId",
                table: "RestaurantIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantIngredients_Restaurants_RestaurantId",
                table: "RestaurantIngredients",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_OwnerId",
                table: "Restaurants",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierIngredients_Ingredients_IngredientId",
                table: "SupplierIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierIngredients_Suppliers_SupplierId",
                table: "SupplierIngredients",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrders_Restaurants_RestaurantId",
                table: "SupplierOrders",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrders_Suppliers_SupplierId",
                table: "SupplierOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Restaurants_RestaurantId",
                table: "Tables",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Restaurants_RestaurantId",
                table: "Users",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
