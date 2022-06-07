using RestaurantBackend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Data.DTOs
{
    public class RestaurantDto
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public AddressDto Address { get; set; }
		public MenuDto Menu { get; set; }
		public EmployeeDto Owner { get; set; }
		public IEnumerable<TableDto> Tables { get; set; }
		public IEnumerable<RestaurantIngredientDto> RestaurantIngredients { get; set; }

		public RestaurantDto() { }

		public RestaurantDto(Restaurant restaurant)
		{
			Id = restaurant.Id;
			Name = restaurant.Name;
			Owner = new EmployeeDto(restaurant.Owner);

			Address = new AddressDto(restaurant.Address);
			Menu = new MenuDto(restaurant.Menu, restaurant.Menu.Dishes);

			var l = new List<RestaurantIngredientDto>();

			foreach (var item in restaurant.Ingredients)
			{
				l.Add(new RestaurantIngredientDto(item));
			}

			RestaurantIngredients = l;

			var t = new List<TableDto>();

			foreach (var item in restaurant.Tables)
			{
				t.Add(new TableDto(item));
			}

			Tables = t;
		}
	}
}
