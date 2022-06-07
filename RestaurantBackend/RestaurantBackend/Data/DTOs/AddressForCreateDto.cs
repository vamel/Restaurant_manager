using RestaurantBackend.Models;
using System;

namespace RestaurantBackend.Data.DTOs
{
	public class AddressForCreateDto
	{
		public int CountryId { get; set; }
		public string PostalCode { get; set; }
		public string City { get; set; }
		public string Street { get; set; }
		public string StreetNumber { get; set; }

		internal Address ToAddress()
		{
			return new Address { CountryId = CountryId, PostalCode = PostalCode, City = City, Street = Street, StreetNumber = StreetNumber };
		}
	}
}