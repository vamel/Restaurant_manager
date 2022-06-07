using RestaurantBackend.Models;
using System;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class DishWithAvailabilityDto
	{
		public int Amount { get; set; }
		public DishDto Dish { get; set; }
	}
}