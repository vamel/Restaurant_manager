using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class OrderIngredientDto
	{
		public int SupplierOrderId { get; set; }
		public int IngredientId { get; set; }
		public float Amount { get; set; }

		public OrderIngredientDto() { }
		public OrderIngredientDto(OrderIngredient ingredient)
		{
			SupplierOrderId = ingredient.SupplierOrderId;
			IngredientId = ingredient.IngredientId;
			Amount = ingredient.Amount;
		}

		public OrderIngredient ToOrderIngredient()
		{
			return new OrderIngredient
			{
				SupplierOrderId = SupplierOrderId,
				IngredientId = IngredientId,
				Amount = Amount
			};
		}
	}
}
