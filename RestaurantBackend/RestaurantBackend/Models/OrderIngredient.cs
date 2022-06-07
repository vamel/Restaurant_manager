namespace RestaurantBackend.Models
{
	public class OrderIngredient
	{
		public int SupplierOrderId { get; set; }
		public SupplierOrder SupplierOrder { get; set; }
		public int IngredientId { get; set; }
		public Ingredient Ingredient { get; set; }
		public float Amount { get; set; }
	}
}
