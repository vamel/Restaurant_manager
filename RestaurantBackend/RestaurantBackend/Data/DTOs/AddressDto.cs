using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class AddressDto
	{
		public int Id { get; set; }
		public CountryDto Country { get; set; }
		public string PostalCode { get; set; }
		public string City { get; set; }
		public string Street { get; set; }
		public string StreetNumber { get; set; }

		public AddressDto() { }

		public AddressDto(Address address)
		{
			Id = address.Id;
			Country = new CountryDto(address.Country);
			City = address.City;
			PostalCode = address.PostalCode;
			Street = address.Street;
			StreetNumber = address.StreetNumber;
		}
	}
}