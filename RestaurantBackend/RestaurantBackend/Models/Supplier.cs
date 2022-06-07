using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class Supplier
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string BankInformation { get; set; }
		public int AddressId { get; set; }
		public Address Address { get; set; }
		public ICollection<SupplierIngredient> Ingredients { get; set; }
		public ICollection<SupplierOrder> SupplierOrders { get; set; }
	}
}
