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
	public class SupplierController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public SupplierController(IRestaurantRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("getSupplier/{id}", Name ="getSupplier")]
		public async Task<IActionResult> GetSupplier(int id)
		{
			var supplier = await _repo.GetSupplier(id);
			if (supplier == null) return NotFound();

			return Ok(new SupplierDto(supplier));
		}

		[HttpGet("getSuppliers", Name = "getSuppliers")]
		public async Task<IActionResult> GetSuppliers()
		{
			var suppliers = _repo.GetSuppliers();

			var l = new List<SupplierDto>();
			foreach (var supplier in suppliers)
				l.Add(new SupplierDto(supplier));

			return Ok(l);
		}


		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("addSupplier")]
		public async Task<IActionResult> AddSupplier([FromBody] SupplierForCreateDto supplier)
		{
			if (supplier.AddressId == null || supplier.AddressId == 0)
			{
				var allAddresses = _repo.GetAllAddresses();
				var matchingAddress = allAddresses.FirstOrDefault(a => a.Street == supplier.Address.Street &&
																a.CountryId == supplier.Address.CountryId &&
																a.PostalCode == supplier.Address.PostalCode &&
																a.City == supplier.Address.City &&
																a.StreetNumber == supplier.Address.StreetNumber);

				if (matchingAddress != null)
					supplier.AddressId = matchingAddress.Id;
				else
				{
					var newAddress = await _repo.Add<Address>(supplier.Address.ToAddress());
					supplier.AddressId = newAddress.Id;
				}
			}
			else if (await _repo.GetAddress(supplier.AddressId.Value) == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Address with id doesn't exist." });


			Supplier newSupplier = new()
			{
				Name = supplier.Name,
				AddressId = supplier.AddressId.Value,
				BankInformation = supplier.BankInformation,
			};

			var result = await _repo.Add(newSupplier);

			return Ok(new SupplierDto(result));
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("updateSupplier")]
		public async Task<IActionResult> UpdateSupplier([FromBody] SupplierForUpdateDto supplier)
		{
			var oldSupplier = await _repo.GetSupplier(supplier.Id);

			if (oldSupplier == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Supplier does not exist." });


			oldSupplier.Name = supplier.Name;
			oldSupplier.BankInformation = supplier.BankInformation;

			_repo.Update(oldSupplier);

			return Ok(new SupplierDto(oldSupplier));
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpDelete("deleteSupplier/{id}")]
		public async Task<IActionResult> DeleteSupplier(int id)
		{
			var supplier = await _repo.GetSupplier(id);

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (!supplier.SupplierOrders.All(so => _repo.IsEmployeeInRestaurant(currentEmployee.Id, so.RestaurantId).Result))
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot delete suppliers that other restaurants use." });

			_repo.Delete(supplier);

			return Ok(new Response { Status = "Success", Message = $"Supplier {id} deleted successfuly."});
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("addSupplierIngredient")]
		public async Task<IActionResult> AddSupplierIngredient([FromBody] SupplierIngredientDto supplierIngredient)
		{
			if (await _repo.GetIngredient(supplierIngredient.IngredientId) == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient doesn't exist. First add it in the ingredients API module instead." });

			if (await _repo.GetSupplier(supplierIngredient.SupplierId) == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Supplier doesn't exist. First add it in the ingredients API module instead." });

			if ((await _repo.GetSupplier(supplierIngredient.SupplierId)).Ingredients.Any(i => i.IngredientId == supplierIngredient.IngredientId))
				return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient already exists in inventory. Use updateRestaurantIngredient instead." });


			var newIngredient = await _repo.Add(supplierIngredient.ToSupplierIngredient());

			return Ok(new SupplierIngredientDto(newIngredient));
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpDelete("deleteSupplierIngredient/{supplierId}/{ingredientId}")]
		public async Task<IActionResult> DeleteSupplierIngredient(int supplierId, int ingredientId)
		{
			if (await _repo.GetSupplier(supplierId) == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Supplier doesn't exist." });

			if (await _repo.GetIngredient(ingredientId) == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient doesn't exist." });

			_repo.Delete(await _repo.GetSupplierIngredient(supplierId, ingredientId));

			return Ok(new Response { Status = "Success", Message = $"Ingredient {ingredientId} removed from supplier {supplierId}'s offerings."});
		}
	}
}
