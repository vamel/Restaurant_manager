using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class CountryDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string CountryCode { get; set; }

		public CountryDto() { }

		public CountryDto(Country country)
		{
			Id = country.Id;
			Name = country.Name;
			CountryCode = country.CountryCode;
		}
	}
}