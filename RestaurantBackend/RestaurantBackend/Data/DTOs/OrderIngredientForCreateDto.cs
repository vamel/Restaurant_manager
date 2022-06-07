using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class OrderIngredientForCreateDto
	{
		public int IngredientId { get; set; }
		public float Amount { get; set; }

		public OrderIngredient ToOrderIngredient()
		{
			return new OrderIngredient
			{
				IngredientId = IngredientId,
				Amount = Amount
			};
		}
	}
}
