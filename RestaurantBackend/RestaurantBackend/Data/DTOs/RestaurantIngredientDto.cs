namespace RestaurantBackend.Models
{
	public class RestaurantIngredientDto
	{
		public int RestaurantId { get; set; }
		public int IngredientId { get; set; }
		public float Amount { get; set; }

		public RestaurantIngredientDto() { }

		public RestaurantIngredientDto(RestaurantIngredient restaurantIngredient)
		{
			RestaurantId = restaurantIngredient.RestaurantId;
			IngredientId = restaurantIngredient.IngredientId;
			Amount = restaurantIngredient.Amount;
		}

		public RestaurantIngredient ToRestaurantIngredient()
		{
			return new RestaurantIngredient { Amount = Amount, IngredientId = IngredientId, RestaurantId = RestaurantId };
		}
	}
}
