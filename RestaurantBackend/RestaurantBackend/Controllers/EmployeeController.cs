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
	public class EmployeeController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public EmployeeController(IRestaurantRepository repo)
		{
			_repo = repo;
		}


		[HttpGet("getEmployee/{id}", Name = "getEmployee")]
		public async Task<IActionResult> GetEmployee(int id)
		{
			var user = await _repo.GetEmployee(id);
			if (user == null) return NotFound();

			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name)).Id;
			if (user.RestaurantId.HasValue && !(await _repo.IsEmployeeInRestaurant(currentUser, user.RestaurantId.Value)))
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "Different restaurant!" });

			var employeeToReturn = new EmployeeDto(user);

			return Ok(employeeToReturn);
		}

		[HttpGet("getEmployeeByName/{name}", Name = "getEmployeeByName")]
		public async Task<IActionResult> GetEmployeeByName(string name)
		{
			var user = await _repo.GetEmployeeByName(name);
			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name)).Id;

			if (user.RestaurantId.HasValue && !(await _repo.IsEmployeeInRestaurant(currentUser, user.RestaurantId.Value)))
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "Different restaurant!" });

			var employeeToReturn = new EmployeeDto(user);

			return Ok(employeeToReturn);
		}

		[HttpGet("getEmployeeRole/{id}", Name = "getEmployeeRole")]
		public async Task<IActionResult> GetEmployeeRole(int id)
		{
			var role = await _repo.GetEmployeeRole(id);
			var employee = await _repo.GetEmployee(id);
			if (role == null) return NotFound();

			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name)).Id;
			if (employee.RestaurantId.HasValue && !(await _repo.IsEmployeeInRestaurant(currentUser, employee.RestaurantId.Value)))
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "Different restaurant!" });

			return Ok(role);
		}

		[HttpGet("getRestaurantEmployees/{restaurantId}", Name = "getRestaurantEmployees")]
		public async Task<IActionResult> GetRestaurantEmployees(int restaurantId)
		{
			var restaurant = await _repo.GetRestaurant(restaurantId);
			if (restaurant == null) return NotFound();

			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name));
			if (!(await _repo.IsEmployeeInRestaurant(currentUser.Id, restaurantId)))
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "Different restaurant!" });


			var employees = _repo.GetRestaurantEmployees(restaurantId);
			List<EmployeeDto> employeeDtos = new();
			foreach (var employee in employees)
				employeeDtos.Add(new EmployeeDto(employee));

			return Ok(employeeDtos);
		}

		[Authorize(Roles = $"{EmployeeRoles.Owner},{EmployeeRoles.Manager}")]
		[HttpPost]
		[Route("updateEmployeeWorkingForYou")]
		public async Task<IActionResult> UpdateEmployeeWorkingForYou([FromBody] EmployeeForUpdateAsManagerDto updatedEmployee)
		{
			var employee = await _repo.GetEmployee(updatedEmployee.Id);
			if (employee == null) return NotFound();

			if (_repo.GetEmployeeRoles(employee.Id).Any(ur => ur == EmployeeRoles.Owner))
                return BadRequest(new Response { Status = "Bad request", Message = "Owners cannot update other owners." });

			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name)).Id;
			if (employee.RestaurantId.HasValue && !(await _repo.IsEmployeeInRestaurant(currentUser, employee.RestaurantId.Value)))
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "Different restaurant!" });

			if (User.IsInRole(EmployeeRoles.Manager) && _repo.GetEmployeeRoles(currentUser).Any(ur => ur == EmployeeRoles.Manager || ur == EmployeeRoles.Owner))
			{
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "Not important enough!" });
			}

			employee.Salary = updatedEmployee.Salary;

			_repo.Update(employee);

			return Ok(new EmployeeDto(employee));
		}

		[HttpPost]
		[Route("updateEmployeeSelf")]
		public async Task<IActionResult> UpdateEmployeeSelf([FromBody] EmployeeForUpdateSelfDto updatedEmployee)
		{
			var employee = await _repo.GetEmployee(updatedEmployee.Id);
			if (employee == null) return NotFound();

			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name)).Id;
			if (employee.Id != currentUser)
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "Don't update other people!" });

			employee.Name = updatedEmployee.Name;
			employee.Surname = updatedEmployee.Surname;
			employee.BirthDate = updatedEmployee.BirthDate;
			employee.PESEL = updatedEmployee.PESEL;

			_repo.Update(employee);

			return Ok(new EmployeeDto(employee));
		}

		[Authorize(Roles = EmployeeRoles.Owner)]
		[HttpDelete("deleteEmployeeWorkingForYou/{id}")]
		public async Task<IActionResult> DeleteEmployeeWorkingForYou(int id)
		{
			var employee = await _repo.GetEmployee(id);
			if (employee == null) return NotFound();

			if (_repo.GetEmployeeRoles(employee.Id).Any(ur => ur == EmployeeRoles.Owner))
				return BadRequest(new Response { Status = "Bad request", Message = "Owners cannot update other owners." });

			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name)).Id;
			if (employee.RestaurantId.HasValue && !(await _repo.IsEmployeeInRestaurant(currentUser, employee.RestaurantId.Value)))
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "Different restaurant!" });


			_repo.Delete(employee);

			return Ok(new Response { Status = "Success", Message = $"Employee {id} deleted."});
		}

		[Authorize(Roles = EmployeeRoles.Owner)]
		[HttpDelete("deleteSelfAsOwner/{id}")]
		public async Task<IActionResult> DeleteSelfAsOwner(int id)
		{
			var employee = await _repo.GetEmployee(id);
			if (employee == null) return NotFound();

			var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name)).Id;
			if (employee.Id != currentUser)
				return Unauthorized(new Response() { Status = "Unauthorized", Message = "You can only delete yourself!" });

			_repo.Delete(employee);

			return Ok(new Response { Status = "Success", Message = $"Owner {id} deleted, along with their restaurant and all their employees. Hope that's what you wanted." });
		}
	}
}
