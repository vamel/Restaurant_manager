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
	public class TableController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public TableController(IRestaurantRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("getTable/{id}", Name ="getTable")]
		public async Task<IActionResult> GetTable(int id)
		{
			var table = await _repo.GetTable(id);
			if (table == null) return NotFound();

			var currentUser = await _repo.GetEmployeeByName(User.Identity.Name);
			if (!(await _repo.IsEmployeeInRestaurant(currentUser.Id, table.RestaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot access table data of a different restaurant." });

			return Ok(new TableDto(table));
		}


		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("addTable")]
		public async Task<IActionResult> AddTable([FromBody] TableForCreateDto table)
		{
			var newTable = table.ToTable();

			var currentUser = await _repo.GetEmployeeByName(User.Identity.Name);
			if (!(await _repo.IsEmployeeInRestaurant(currentUser.Id, table.RestaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot add table data to a different restaurant." });

			newTable = await _repo.Add(newTable);

			return Ok(new TableDto(newTable));
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("updateTable")]
		public async Task<IActionResult> UpdateTable([FromBody] TableDto table)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);
			if (!(await _repo.IsEmployeeInRestaurant(currentEmployee.Id, table.RestaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Not your table." });

			var oldTable = await _repo.GetTable(table.Id);
			if (oldTable == null)
				return BadRequest(new Response { Status = "Bad Request", Message = "Table does not exist." });

			if (table.RestaurantId != oldTable.RestaurantId)
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot change table restaurant." });

			oldTable.SeatCount = table.SeatCount;
			oldTable.Name = table.Name;

			_repo.Update(oldTable);

			return Ok(new TableDto(oldTable));
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpDelete("deleteTable/{id}")]
		public async Task<IActionResult> DeleteTable(int id)
		{
			var table = await _repo.GetTable(id);
			if (table == null) return NotFound();

			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (!(await _repo.IsEmployeeInRestaurant(currentEmployee.Id, table.RestaurantId)))
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot delete other restaurant tables." });

			_repo.Delete(table);

			return Ok(new Response { Status = "Success", Message = $"Table {id} deleted successfuly."});
		}
	}
}
