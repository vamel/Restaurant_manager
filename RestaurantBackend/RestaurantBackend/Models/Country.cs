using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class Country
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string CountryCode { get; set; }
		public ICollection<Address> Addresses { get; set; }
	}
}
