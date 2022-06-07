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
	public class IngredientController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public IngredientController(IRestaurantRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("getIngredient/{id}", Name ="getIngredient")]
		public async Task<IActionResult> GetIngredient(int id)
		{
			var ingredient = await _repo.GetIngredient(id);
			if (ingredient == null) return NotFound();

			return Ok(new IngredientDto(ingredient));
		}

		[HttpGet("getIngredients", Name = "getIngredients")]
		public async Task<IActionResult> GetIngredients()
		{
			var ingredienst = _repo.GetIngredients();

			var l = new List<IngredientDto>();

			foreach (var ingredient in ingredienst)
				l.Add(new IngredientDto(ingredient));

			return Ok(l);
		}


		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("addIngredient")]
		public async Task<IActionResult> AddIngredient([FromBody] IngredientForCreateDto ingredient)
		{
			var newIngredient = ingredient.ToIngredient();

			await _repo.Add(newIngredient);

			return Ok(new IngredientDto(newIngredient));
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("updateIngredient")]
		public async Task<IActionResult> UpdateIngredient([FromBody] IngredientDto ingredient)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			var oldIngredient = await _repo.GetIngredient(ingredient.Id);
			if (oldIngredient == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient does not exist." });

			if (!oldIngredient.RestaurantIngredients.Any(ri => _repo.IsEmployeeInRestaurant(currentEmployee.Id, ri.RestaurantId).Result))
				return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient not used by your restaurant." });

			oldIngredient.Price = ingredient.Price;
			oldIngredient.IsPricePerKilogram = ingredient.IsPricePerKilogram;
			oldIngredient.Name = ingredient.Name;

			_repo.Update(oldIngredient);

			return Ok(new IngredientDto(oldIngredient));
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpDelete("deleteIngredient/{id}")]
		public async Task<IActionResult> DeleteIngredient(int id)
		{
			var ingredient = await _repo.GetIngredient(id);

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (!ingredient.RestaurantIngredients.All(ri => _repo.IsEmployeeInRestaurant(currentEmployee.Id, ri.RestaurantId).Result))
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot delete ingredients that other restaurants use." });

			_repo.Delete(ingredient);

			return Ok(new Response { Status = "Success", Message = $"Ingredient {id} deleted successfuly."});
		}
	}
}
