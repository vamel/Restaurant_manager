using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class AddressForUpdateDto
	{
		public int Id { get; set; }
		public int CountryId { get; set; }
		public string PostalCode { get; set; }
		public string City { get; set; }
		public string Street { get; set; }
		public string StreetNumber { get; set; }

		public AddressForUpdateDto() { }

		public AddressForUpdateDto(Address address)
		{
			Id = address.Id;
			CountryId = address.CountryId;
			City = address.City;
			PostalCode = address.PostalCode;
			Street = address.Street;
			StreetNumber = address.StreetNumber;
		}
	}
}