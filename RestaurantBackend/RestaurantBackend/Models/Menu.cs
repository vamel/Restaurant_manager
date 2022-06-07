using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class Menu
	{
		public int Id { get; set; }
		public ICollection<Dish> Dishes { get; set; }
	}
}
