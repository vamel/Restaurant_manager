using System;
using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class SupplierOrder
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public int SupplierId { get; set; }
		public Supplier Supplier { get; set; }
		public int RestaurantId { get; set; }
		public Restaurant Restaurant { get; set; }
		public ICollection<OrderIngredient> OrderIngredients { get; set; }
	}
}
