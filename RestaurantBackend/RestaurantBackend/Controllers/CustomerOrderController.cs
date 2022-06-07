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
	public class CustomerOrderController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public CustomerOrderController(IRestaurantRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("getOrder/{id}", Name ="getOrder")]
		public async Task<IActionResult> GetOrder(int id)
		{
			var order = await _repo.GetCustomerOrder(id);
			if (order == null) return NotFound();

			return Ok(new CustomerOrderDto(order));
		}

		[HttpGet("getOrders/{restaurantId}", Name = "getOrders")]
		public async Task<IActionResult> GetOrders(int restaurantId)
		{
			var orders = _repo.GetCustomerOrders(restaurantId);

			var l = new List<CustomerOrderDto>();
			foreach (var item in orders)
				l.Add(new CustomerOrderDto(item));

			return Ok(l);
		}

		[HttpPost]
		[Route("addOrder")]
		public async Task<IActionResult> AddOrder([FromBody] CustomerOrderForCreateDto customerOrder)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (!(await _repo.IsEmployeeInRestaurant(currentEmployee.Id, customerOrder.RestaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Not your restaurant." });

			var assignedEmployee = await _repo.GetEmployee(customerOrder.AssignedEmployeeId);

			if (assignedEmployee == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Assigned employee does not exist." });

			var newCustomerOrder = customerOrder.ToCustomerOrder();

			newCustomerOrder = await _repo.Add(newCustomerOrder);

			var restaurant = await _repo.GetRestaurant(customerOrder.RestaurantId);

			foreach (var item in customerOrder.Dishes)
			{
				var newDishOrder = item.ToOrderDish();

				if (!(restaurant.Menu.Dishes.Any(d => d.Id == newDishOrder.DishId)))
					return BadRequest(new Response { Status = "Bad Request", Message = "Dish does not exist." });

				newDishOrder.OrderId = newCustomerOrder.Id;

				await _repo.Add(newDishOrder);

				foreach (var ingredient in _repo.GetDishIngredients(item.DishId))
				{
					var restaurantIngredient = await _repo.GetRestaurantIngredient(customerOrder.RestaurantId, ingredient.IngredientId);

					if (restaurantIngredient != null)
					{
						restaurantIngredient.Amount -= item.Count * ingredient.Amount;

						_repo.Update(restaurantIngredient);
					}
					else
					{
						restaurantIngredient = new RestaurantIngredient
						{
							RestaurantId = customerOrder.RestaurantId,
							IngredientId = ingredient.IngredientId,
							Amount = 0
						};

						await _repo.Add(restaurantIngredient);
					}
				}
			}


			return Ok(new CustomerOrderDto(await _repo.GetCustomerOrder(newCustomerOrder.Id)));
		}
	}
}
