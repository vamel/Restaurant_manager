namespace RestaurantBackend.Models
{
	public class SupplierIngredient
	{
		public int SupplierId { get; set; }
		public Supplier Supplier { get; set; }
		public int IngredientId { get; set; }
		public Ingredient Ingredient { get; set; }
	}
}
