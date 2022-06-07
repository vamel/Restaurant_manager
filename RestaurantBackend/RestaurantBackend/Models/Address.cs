using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class Address
	{
		public int Id { get; set; }
		public int CountryId { get; set; }
		public Country Country { get; set; }
		public string PostalCode { get; set; }
		public string City { get; set; }
		public string Street { get; set; }
		public string StreetNumber { get; set; }
		public ICollection<Supplier> Suppliers { get; set; }
		public ICollection<Restaurant> Restaurant { get; set; }
	}
}
