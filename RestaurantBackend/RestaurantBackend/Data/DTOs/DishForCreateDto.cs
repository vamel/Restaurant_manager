using RestaurantBackend.Models;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class DishForCreateDto
	{
		public string Name { get; set; }
		public int Price { get; set; }
		public int MenuId { get; set; }
		public IEnumerable<DishIngredientForDishIngredientsDto> DishIngredients { get; set; }

		public Dish ToDish()
		{
			var dish = new Dish()
			{
				Name = Name,
				Price = Price,
				MenuId = MenuId
			};
			return dish;
		}
	}
}