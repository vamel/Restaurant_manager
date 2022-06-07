using RestaurantBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantBackend.Data.DTOs
{
	public class MenuWithAvailabilityDto
	{
		public int Id { get; set; }
		public IEnumerable<DishWithAvailabilityDto> Dishes { get; set; }

		public MenuWithAvailabilityDto() { }

		public MenuWithAvailabilityDto(Menu menu, IEnumerable<Dish> dishes, Restaurant restaurant)
		{
			Id = menu.Id;

			var menuDishes = new List<DishWithAvailabilityDto>();

			if (dishes != null)
			{
				foreach (var dish in dishes)
				{
					int availableCount = int.MaxValue;
					
					if (dish.DishIngredients.Count > 0)
					{
						availableCount = restaurant.Ingredients
							.Where(i => dish.DishIngredients
								.Any(di => di.IngredientId == i.IngredientId))
							.Min(i => (int)(i.Amount / dish.DishIngredients
								.FirstOrDefault(di => di.IngredientId == i.IngredientId).Amount));
					}
					

					menuDishes.Add(new DishWithAvailabilityDto { Amount = availableCount, Dish = new DishDto(dish) });
				}
			}

			Dishes = menuDishes;
		}
	}
}