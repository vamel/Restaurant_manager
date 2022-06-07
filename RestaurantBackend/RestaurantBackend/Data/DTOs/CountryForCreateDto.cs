using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class CountryForCreateDto
	{
		public string Name { get; set; }
		public string CountryCode { get; set; }

		public Country ToCountry()
		{
			return new Country { Name = Name, CountryCode = CountryCode };
		}
	}
}