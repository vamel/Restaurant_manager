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
	public class RestaurantController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public RestaurantController(IRestaurantRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("getRestaurant/{id}", Name = "getRestaurant")]
		public async Task<IActionResult> GetRestaurant(int id)
		{
			var restaurant = await _repo.GetRestaurant(id);
			if (restaurant == null)
				return NotFound();
			var restaurantToReturn = new RestaurantDto(restaurant);

			return Ok(restaurantToReturn);
		}

		[HttpGet("getOwnedRestaurants/{ownerId}", Name = "getOwnedRestaurants")]
		public async Task<IActionResult> GetOwnedRestaurants(int ownerId)
		{
			var restaurants = _repo.GetOwnedRestaurants(ownerId);

			var l = new List<RestaurantDto>();
			foreach (var restaurant in restaurants)
				l.Add(new RestaurantDto(restaurant));

			return Ok(l);
		}

		[Authorize(Roles = EmployeeRoles.Owner)]
		[HttpPost()]
		[Route("createRestaurant")]
		public async Task<IActionResult> CreateRestaurant([FromBody] RestaurantForCreateDto restaurant)
		{
			var allAddresses = _repo.GetAllAddresses();
			var matchingAddress = allAddresses.FirstOrDefault(a => a.Street == restaurant.Address.Street &&
															a.CountryId == restaurant.Address.CountryId &&
															a.PostalCode == restaurant.Address.PostalCode &&
															a.City == restaurant.Address.City &&
															a.StreetNumber == restaurant.Address.StreetNumber);
			int addressId;

			if (matchingAddress != null)
				addressId = matchingAddress.Id;
			else
			{
				var newAddress = await _repo.Add<Address>(restaurant.Address.ToAddress());
				addressId = newAddress.Id;
			}

			int menuId;

			if (restaurant.ExistingMenuId == null || restaurant.ExistingMenuId == -1)
			{
				menuId = (await _repo.Add<Menu>(new Menu())).Id;
			}
			else
			{
				menuId = restaurant.ExistingMenuId.Value;
			}

			Restaurant newRestaurant = new()
			{
				Name = restaurant.Name,
				AddressId = addressId,
				OwnerId = restaurant.OwnerId,
				MenuId = menuId,
			};

			var result = await _repo.Add(newRestaurant);

			return Ok(new RestaurantDto(result));
		}

		[Authorize(Roles = EmployeeRoles.Owner)]
		[HttpPost]
		[Route("updateRestaurant")]
		public async Task<IActionResult> UpdateRestaurant([FromBody] RestaurantForUpdateDto restaurant)
		{
			var oldRestaurant = await _repo.GetRestaurant(restaurant.Id);

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (oldRestaurant.OwnerId != currentEmployee.Id)
				return BadRequest(new Response { Status = "Bad Request", Message = "Not your restaurant." });

			oldRestaurant.Name = restaurant.Name;

			oldRestaurant = _repo.Update(oldRestaurant);

			return Ok(new RestaurantDto(oldRestaurant));
		}

		[Authorize(Roles = EmployeeRoles.Owner)]
		[HttpDelete("deleteRestaurant/{id}")]
		public async Task<IActionResult> DeleteRestaurant(int id)
		{
			var restaurant = await _repo.GetRestaurant(id);

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (restaurant.OwnerId != currentEmployee.Id)
				return BadRequest(new Response { Status = "Bad Request", Message = "Not your restaurant." });

			_repo.Delete(restaurant);

			return Ok(new Response { Status = "Bad Request", Message = $"Restaurant {id} deleted." });
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("addRestaurantIngredient")]
		public async Task<IActionResult> AddRestaurantIngredient([FromBody] RestaurantIngredientDto restaurantIngredient)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);


			if (await _repo.IsEmployeeInRestaurant(currentEmployee.Id, restaurantIngredient.RestaurantId))
			{
				if (_repo.GetIngredient(restaurantIngredient.IngredientId) == null)
					return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient doesn't exist. First add it in the ingredients API module instead." });


				if (_repo.GetRestaurantIngredientsByRestaurant(restaurantIngredient.RestaurantId).Any(ri => ri.IngredientId == restaurantIngredient.RestaurantId))
					return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient already exists in inventory. Use updateRestaurantIngredient instead." });


				var newIngredient = await _repo.Add(restaurantIngredient.ToRestaurantIngredient());

				return Ok(new RestaurantIngredientDto(newIngredient));
			}
			else
			{
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot add to other restaurant inventories." });
			}
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("updateRestaurantIngredient")]
		public async Task<IActionResult> UpdateRestaurantIngredient([FromBody] RestaurantIngredientDto restaurantIngredient)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);


			if (await _repo.IsEmployeeInRestaurant(currentEmployee.Id, restaurantIngredient.RestaurantId))
			{
				var oldRI = await _repo.GetRestaurantIngredient(restaurantIngredient.RestaurantId, restaurantIngredient.IngredientId);

				if (oldRI == null)
					return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient doesn't exist in restaurant inventory. First add it in using addRestaurantIngredient." });

				oldRI.Amount = restaurantIngredient.Amount;

				_repo.Update(oldRI);

				return Ok(new RestaurantIngredientDto(oldRI));
			}
			else
			{
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot add to other restaurant inventories." });
			}
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpDelete("deleteRestaurantIngredient/{restaurantId}/{ingredientId}")]
		public async Task<IActionResult> DeleteRestaurantIngredient(int restaurantId, int ingredientId)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (await _repo.IsEmployeeInRestaurant(currentEmployee.Id, restaurantId))
			{
				var ri = await _repo.GetRestaurantIngredient(restaurantId, ingredientId);

				if (ri == null)
					return BadRequest(new Response { Status = "Bad Request", Message = "Ingredient doesn't exist in restaurant inventory. Nothing deleted." });

				_repo.Delete(ri);

				return Ok(new Response { Status = "Success", Message = $"Ingredient {ingredientId} deleted from the inventory of restaurant {restaurantId}" });
			}
			else
			{
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot delete from other restaurant inventories." });
			}
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("addSupplierOrder")]
		public async Task<IActionResult> AddSupplierOrder([FromBody] SupplierOrderForCreateDto supplierOrder)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (!(await _repo.IsEmployeeInRestaurant(currentEmployee.Id, supplierOrder.RestaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Not your restaurant." });

			var supplier = await _repo.GetSupplier(supplierOrder.SupplierId);

			if (supplier == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Supplier does not exist." });

			var newSupplierOrder = supplierOrder.ToSupplierOrder();

			newSupplierOrder = await _repo.Add(newSupplierOrder);


			foreach (var item in supplierOrder.OrderIngredients)
			{
				var newOrderIngredient = item.ToOrderIngredient();

				if (!(supplier.Ingredients.Any(i => i.IngredientId == item.IngredientId)))
					return BadRequest(new Response { Status = "Bad Request", Message = "Supplier does not offer this ingredient." });

				newOrderIngredient.SupplierOrderId = newSupplierOrder.Id;

				await _repo.Add(newOrderIngredient);

				var restaurantIngredient = await _repo.GetRestaurantIngredient(supplierOrder.RestaurantId, item.IngredientId);

				if (restaurantIngredient != null)
				{
					restaurantIngredient.Amount += newOrderIngredient.Amount;

					_repo.Update(restaurantIngredient);
				}
				else
				{
					restaurantIngredient = new RestaurantIngredient
					{
						RestaurantId = newSupplierOrder.RestaurantId,
						IngredientId = item.IngredientId,
						Amount = newOrderIngredient.Amount
					};

					await _repo.Add(restaurantIngredient);
				}
			}


			return Ok(new SupplierOrderDto(await _repo.GetSupplierOrder(newSupplierOrder.Id)));
		}

		[HttpGet("getSupplierOrder/{id}")]
		public async Task<IActionResult> GetSupplierOrder(int id)
		{
			var supplierOrder = await _repo.GetSupplierOrder(id);
			if (supplierOrder == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Supplier order not found." });

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);
			if (!(await _repo.IsEmployeeInRestaurant(currentEmployee.Id, supplierOrder.RestaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Not your restaurant." });


			return Ok(new SupplierOrderDto(supplierOrder));
		}

		[HttpGet("getSupplierOrdersTo/{restaurantId}")]
		public async Task<IActionResult> GetSupplierOrdersTo(int restaurantId)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);
			if (!(await _repo.IsEmployeeInRestaurant(currentEmployee.Id, restaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Not your restaurant." });

			var supplierOrders = _repo.GetRestaurantSupplierOrders(restaurantId);

			var l = new List<SupplierOrderDto>();
			foreach (var item in supplierOrders)
				l.Add(new SupplierOrderDto(item));

			return Ok(l);
		}

		[HttpGet("getSupplierOrdersToFrom/{restaurantId}/{supplierId}")]
		public async Task<IActionResult> GetSupplierOrdersToFrom(int restaurantId, int supplierId)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);
			if (!(await _repo.IsEmployeeInRestaurant(currentEmployee.Id, restaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Not your restaurant." });

			var supplierOrders = _repo.GetRestaurantSupplierOrders(restaurantId).Where(so => so.SupplierId == supplierId);

			var l = new List<SupplierOrderDto>();
			foreach (var item in supplierOrders)
				l.Add(new SupplierOrderDto(item));

			return Ok(l);
		}
	}
}
