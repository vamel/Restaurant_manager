namespace RestaurantBackend.Models
{
	public class RestaurantIngredient
	{
		public int RestaurantId { get; set; }
		public Restaurant Restaurant { get; set; }
		public int IngredientId { get; set; }
		public Ingredient Ingredient { get; set; }
		public float Amount { get; set; }
	}
}
