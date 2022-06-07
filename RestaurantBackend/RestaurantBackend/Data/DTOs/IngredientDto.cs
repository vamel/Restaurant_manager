using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class IngredientDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsPricePerKilogram { get; set; }
		public int Price { get; set; }

		public IngredientDto() { }

		public IngredientDto(Ingredient ingredient)
		{
			Id = ingredient.Id;
			Name = ingredient.Name;
			IsPricePerKilogram = ingredient.IsPricePerKilogram;
			Price = ingredient.Price;
		}

		public Ingredient ToIngredient()
		{
			return new Ingredient { Id = Id, Name = Name, IsPricePerKilogram = IsPricePerKilogram, Price = Price };
		}
	}
}
