using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class Dish
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public int MenuId { get; set; }
		public Menu Menu { get; set; }
		public ICollection<DishIngredient> DishIngredients { get; set; }
		public ICollection<OrderDish> Orders { get; set; }
	}
}
