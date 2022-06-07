using RestaurantBackend.Models;
using System;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class DishDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public int MenuId { get; set; }
		public IEnumerable<DishIngredientForDishIngredientsDto> DishIngredients { get; set; }

		public DishDto() { }

		public DishDto(Dish dish)
		{
			Id = dish.Id;
			Name = dish.Name;
			Price = dish.Price;
			MenuId = dish.MenuId;
			var dishIngredients = new List<DishIngredientForDishIngredientsDto>();

			if (dish.DishIngredients != null)
			{
				foreach (var ingredient in dish.DishIngredients)
				{
					dishIngredients.Add(new DishIngredientForDishIngredientsDto()
					{
						IngredientId = ingredient.IngredientId,
						Amount = ingredient.Amount,
					});
				}
			}
			

			DishIngredients = dishIngredients;
		}

		public Dish ToDish()
		{
			var dish = new Dish
			{
				Id = Id,
				Name = Name,
				Price = Price,
				MenuId = MenuId
			};

			return dish;
		}
	}
}