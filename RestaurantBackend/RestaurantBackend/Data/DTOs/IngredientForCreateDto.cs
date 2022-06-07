using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class IngredientForCreateDto
	{
		public string Name { get; set; }
		public bool IsPricePerKilogram { get; set; }
		public int Price { get; set; }

		public Ingredient ToIngredient()
		{
			return new Ingredient { Name = Name, Price = Price, IsPricePerKilogram = IsPricePerKilogram };
		}
	}
}
