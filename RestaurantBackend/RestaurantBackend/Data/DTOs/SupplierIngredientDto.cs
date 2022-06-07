using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class SupplierIngredientDto
	{
		public int SupplierId { get; set; }
		public int IngredientId { get; set; }

		public SupplierIngredientDto() { }

		public SupplierIngredientDto(SupplierIngredient ingredient)
		{
			SupplierId = ingredient.SupplierId;
			IngredientId = ingredient.IngredientId;
		}

		public SupplierIngredient ToSupplierIngredient()
		{
			return new SupplierIngredient
			{
				SupplierId = SupplierId,
				IngredientId = IngredientId
			};
		}
	}
}
