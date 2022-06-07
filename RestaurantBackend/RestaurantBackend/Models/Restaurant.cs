using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class Restaurant
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int AddressId { get; set; }
		public Address Address { get; set; }
		public int MenuId { get; set; }
		public Menu Menu { get; set; }
		public int OwnerId { get; set; }
		public Employee Owner { get; set; }
		public ICollection<Table> Tables { get; set; }
		public ICollection<Employee> Employees { get; set; }
		public ICollection<RestaurantIngredient> Ingredients { get; set; }
		public ICollection<CustomerOrder> CustomerOrders { get; set; }
		public ICollection<SupplierOrder> SupplierOrders { get; set; }
	}
}
