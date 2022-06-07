using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class Ingredient
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsPricePerKilogram { get; set; }
		public int Price { get; set; }
		public ICollection<DishIngredient> DishIngredients { get; set; }
		public ICollection<RestaurantIngredient> RestaurantIngredients { get; set; }
		public ICollection<OrderIngredient> SupplierOrders { get; set; }
		public ICollection<SupplierIngredient> SupplierIngredients { get; set; }
	}
}
