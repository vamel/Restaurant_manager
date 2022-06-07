using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantBackend.Data;
using RestaurantBackend.Data.DTOs;
using RestaurantBackend.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace RestaurantBackend.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class AddressController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public AddressController(IRestaurantRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("getAddress/{id}", Name = "getAddress")]
		public async Task<IActionResult> GetAddress(int id)
		{
			var address = await _repo.GetAddress(id);
			if (address == null) return NotFound();

			var addressToReturn = new AddressDto(address);

			return Ok(addressToReturn);
		}

		[HttpGet("getCountry/{id}", Name = "getCountry")]
		public async Task<IActionResult> GetCountry(int id)
		{
			var country = await _repo.GetCountry(id);
			if (country == null) return NotFound();

			return Ok(new CountryDto(country));
		}

		[Authorize(Roles = EmployeeRoles.Admin)]
		[HttpPost]
		[Route("createCountry")]
		public async Task<IActionResult> CreateCountry([FromBody] CountryForCreateDto country)
		{
			var result = await _repo.Add(country.ToCountry());

			return Ok(new CountryDto(result));
		}

		[Authorize(Roles = EmployeeRoles.Admin)]
		[HttpPost]
		[Route("updateCountry")]
		public async Task<IActionResult> UpdateCountry([FromBody] CountryDto updatedCountry)
		{
			var country = await _repo.GetCountry(updatedCountry.Id);
			if (country == null) return NotFound();

			country.Name = updatedCountry.Name;
			country.CountryCode = updatedCountry.CountryCode;

			_repo.Update(country);

			return Ok(new CountryDto(country));
		}

		[HttpGet("getAllCountries", Name = "getAllCountries")]
		public IActionResult GetAllCountries()
		{
			List<CountryDto> countries = new();

			foreach (var country in _repo.GetAllCountries())
				countries.Add(new CountryDto(country));

			return Ok(countries);
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost("updateAddress")]
		public async Task<IActionResult> UpdateAddress([FromBody] AddressForUpdateDto updatedAddress)
		{
			var address = await _repo.GetAddress(updatedAddress.Id);
			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name));

			if (address == null) return NotFound();

			if (address.Restaurant.Any(r => r.Id == currentUser.RestaurantId || (!currentUser.RestaurantId.HasValue && r.OwnerId == currentUser.Id)))
			{
				address.CountryId = updatedAddress.CountryId;
				address.City = updatedAddress.City;
				address.PostalCode = updatedAddress.PostalCode;
				address.Street = updatedAddress.Street;
				address.StreetNumber = updatedAddress.StreetNumber;

				_repo.Update(address);

				return Ok(new AddressDto(address));
			}
			else if (address.Suppliers.Any(s => s.SupplierOrders.Any(so => so.RestaurantId == currentUser.RestaurantId || (!currentUser.RestaurantId.HasValue && so.Restaurant.OwnerId == currentUser.Id))))
			{
				address.CountryId = updatedAddress.CountryId;
				address.City = updatedAddress.City;
				address.PostalCode= updatedAddress.PostalCode;
				address.Street = updatedAddress.Street;
				address.StreetNumber = updatedAddress.StreetNumber;

				_repo.Update(address);

				return Ok(new AddressDto(address));
			}
			else
			{
				return BadRequest(new Response { Status = "Bad Request", Message = "Not an address of any restaurant relevant to you" });
			}
		}
	}
}
