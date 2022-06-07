using RestaurantBackend.Models;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class MenuDto
	{
		public int Id { get; set; }
		public IEnumerable<DishDto> Dishes { get; set; }

		public MenuDto() { }

		public MenuDto(Menu menu, IEnumerable<Dish> dishes)
		{
			Id = menu.Id;

			var menuDishes = new List<DishDto>();

			if (dishes != null)
			{
				foreach (var dish in dishes)
				{
					menuDishes.Add(new DishDto(dish));
				}
			}

			Dishes = menuDishes;
		}
	}
}