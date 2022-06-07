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
	public class MenuController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public MenuController(IRestaurantRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("getMenu/{id}", Name ="getMenu")]
		//[Route("getRestaurant")]
		public async Task<IActionResult> GetMenu(int id)
		{
			var menu = await _repo.GetMenu(id);
			if (menu == null) return NotFound();

			var dishes = _repo.GetMenuDishes(id);
			var menuToReturn = new MenuDto(menu, dishes);

			return Ok(menuToReturn);
		}

		[HttpGet("getMenuWithDishAvailability/{restaurantId}/{menuId}", Name = "getMenuWithDishAvailability")]
		//[Route("getRestaurant")]
		public async Task<IActionResult> GetMenuWithDishAvailability(int restaurantId, int menuId)
		{
			var menu = await _repo.GetMenu(menuId);
			if (menu == null) return NotFound();

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (!(await _repo.IsEmployeeInRestaurant(currentEmployee.Id, restaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Can't access other restaurant menus." });

			if ((currentEmployee.RestaurantId.HasValue && currentEmployee.Restaurant.MenuId != menuId) && !currentEmployee.OwnedRestaurants.Any(r => r.MenuId == menuId))
				return BadRequest(new Response { Status = "Bad Request", Message = "Can't access other restaurant menus." });

			var dishes = _repo.GetMenuDishes(menuId);
			var menuToReturn = new MenuWithAvailabilityDto(menu, dishes, await _repo.GetRestaurant(restaurantId));

			return Ok(menuToReturn);
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("addDish")]
		public async Task<IActionResult> AddMenuDish([FromBody] DishForCreateDto dish)
		{
			var newDish = dish.ToDish();

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);
			if ((currentEmployee.RestaurantId.HasValue && currentEmployee.Restaurant.MenuId != newDish.MenuId) && !currentEmployee.OwnedRestaurants.Any(r => r.MenuId == newDish.MenuId))
				return BadRequest(new Response { Status = "Bad Request", Message = "Can't add dishes to other restaurant menus." });

			

			await _repo.Add(newDish);


			

			foreach (var item in dish.DishIngredients)
			{
				if (currentEmployee.RestaurantId.HasValue)
				{
					if (!currentEmployee.Restaurant.Ingredients.Any(i => i.IngredientId == item.IngredientId))
						return BadRequest(new Response { Status = "Bad Request", Message = "Dish ingredient not in inventory." });
				}
				else
				{
					if (!currentEmployee.OwnedRestaurants.Any(or => or.Ingredients.Any(i => i.IngredientId == item.IngredientId)))
						return BadRequest(new Response { Status = "Bad Request", Message = "Dish ingredient not in inventory of any owned restaurants." });
				}

				await _repo.Add(new DishIngredient() { IngredientId = item.IngredientId, DishId = newDish.Id, Amount = item.Amount });
			}

			return Ok(new DishDto(await _repo.GetDish(newDish.Id)));
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("updateDish")]
		public async Task<IActionResult> UpdateMenuDish([FromBody] DishDto dish)
		{
			var newDish = dish.ToDish();

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);
			if ((currentEmployee.RestaurantId.HasValue && currentEmployee.Restaurant.MenuId != newDish.MenuId) || currentEmployee.OwnedRestaurants.Any(r => r.MenuId == newDish.MenuId))
			{
				var oldDish = await _repo.GetDish(newDish.Id);

				if (newDish.MenuId != oldDish.MenuId)
					return BadRequest(new Response { Status = "Bad Request", Message = "Cannot move menus." });

				foreach (var item in dish.DishIngredients)
				{
					var oldIngredient = oldDish.DishIngredients.FirstOrDefault(di => di.DishId == dish.Id && di.IngredientId == item.IngredientId);

					if (oldIngredient != null)
					{
						oldIngredient.Amount = item.Amount;

						_repo.Update(oldIngredient);
					}
					else
					{
						await _repo.Add(new DishIngredient() { IngredientId = item.IngredientId, DishId = dish.Id, Amount = item.Amount });
					}
				}

				oldDish.Name = newDish.Name;
				oldDish.Price = newDish.Price;

				_repo.Update(oldDish);

				return Ok(new DishDto(await _repo.GetDish(newDish.Id))); // so dish ingredients are included
			}

			return BadRequest(new Response { Status = "Bad Request", Message = "Can't update dishes of other restaurants menu." });
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpDelete("deleteDish/{dishId}")]
		public async Task<IActionResult> DeleteDish(int dishId)
		{

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);
			if (currentEmployee.RestaurantId.HasValue && currentEmployee.Restaurant.Menu.Dishes.Any(d => d.Id == dishId))
			{
				var dish = await _repo.GetDish(dishId);

				foreach (var item in dish.DishIngredients)
				{
					_repo.Delete(item);
				}

				_repo.Delete(dish);

				return Ok(new Response { Status = "Success", Message = $"Dish {dishId} deleted." });
			}
			else if (!currentEmployee.RestaurantId.HasValue && currentEmployee.OwnedRestaurants.Any(r => r.Menu.Dishes.Any(d => d.Id == dishId)))
			{
				var dish = await _repo.GetDish(dishId);

				foreach (var item in dish.DishIngredients)
				{
					_repo.Delete(item);
				}

				_repo.Delete(dish);

				return Ok(new Response { Status = "Success", Message = $"Dish {dishId} deleted." });
			}

			return BadRequest(new Response { Status = "Bad Request", Message = "Can't delete dishes from other restaurant menus." });
		}
	}
}
