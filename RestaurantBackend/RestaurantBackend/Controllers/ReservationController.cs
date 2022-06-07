using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantBackend.Data;
using RestaurantBackend.Data.DTOs;
using RestaurantBackend.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace RestaurantBackend.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class ReservationController : ControllerBase
	{
		private readonly IRestaurantRepository _repo;
		public ReservationController(IRestaurantRepository repo)
		{
			_repo = repo;
		}

		[HttpGet("getReservation/{id}", Name ="getReservation")]
		public async Task<IActionResult> GetReservation(int id)
		{
			var reservation = await _repo.GetReservation(id);
			if (reservation == null) return NotFound();

			if (IsUserAllowedToAccessReservation(reservation))
			{
				return Ok(new ReservationDto(reservation));
			}
			else return BadRequest(new Response { Status = "Bad Request", Message = "Cannot access reservation data of a different restaurant." });
		}


		[HttpPost]
		[Route("addReservation")]
		public async Task<IActionResult> AddReservation([FromBody] ReservationForCreateDto reservation)
		{
			var newReservation = reservation.ToReservation();

			if (IsUserAllowedToAccessReservation(newReservation))
			{
				if (CheckForOverlap(newReservation))
					return BadRequest(new Response { Status = "Bad Request", Message = "Cannot add a reservation overlapping existing one." });

				newReservation = await _repo.Add(newReservation);
				return Ok(new ReservationDto(newReservation));
			}
			else return BadRequest(new Response { Status = "Bad Request", Message = "Cannot add a reservation to a different restaurant." });
		}

		[HttpPost]
		[Route("updateReservation")]
		public async Task<IActionResult> UpdateReservation([FromBody] ReservationDto reservation)
		{
			var oldReservation = await _repo.GetReservation(reservation.Id);
			if (oldReservation == null)
				return NotFound(new Response { Status = "Not Found", Message = $"Reservation {reservation.Id} not found." });


			oldReservation.StartTime = reservation.StartTime;
			oldReservation.EndTime = reservation.EndTime;
			oldReservation.Name = reservation.Name;
			oldReservation.TableId = reservation.TableId;

			if (!IsUserAllowedToAccessReservation(oldReservation))
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot make changes to another restaurants' reservations." });


			if (CheckForOverlap(oldReservation))
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot make reservation overlap another one." });


			_repo.Update(oldReservation);

			return Ok(new ReservationDto(oldReservation));
		}

		[HttpDelete("deleteReservation/{id}")]
		public async Task<IActionResult> DeleteReservation(int id)
		{
			var reservation = await _repo.GetReservation(id);
			if (reservation == null) return NotFound();

			if(!IsUserAllowedToAccessReservation(reservation))
				return BadRequest(new Response { Status = "Bad Request", Message = "Cannot delete other restaurants' reservations." });

			_repo.Delete(reservation);

			return Ok(new Response { Status = "Success", Message = $"Reservation {id} deleted successfuly."});
		}


		[HttpPost]
		[Route("getFreeTablesDuring")]
		public async Task<IActionResult> GetFreeTablesDuring([FromBody] ReservationQueryDto reservationQuery)
		{
			var currentEmployee = await _repo.GetEmployeeByName(User.Identity.Name);

			if (await _repo.IsEmployeeInRestaurant(currentEmployee.Id, reservationQuery.RestaurantId))
			{
				var freeTables = _repo.GetRestaurantTables(reservationQuery.RestaurantId)
							.Where(t => t.Reservations.All(r =>
								!(reservationQuery.StartTime < r.EndTime && reservationQuery.EndTime > r.StartTime)));
				return Ok(new
				{
					TableIds = freeTables
						.Select(t => t.Id),
					TableNames = freeTables
						.Select(t => t.Name)
				});
			}
			else return BadRequest(new Response { Status = "Bad Request", Message = "Not your restaurant." });
		}

		private bool CheckForOverlap(Reservation reservation)
		{
			var currentEmployee = _repo.GetEmployeeByName(User.Identity.Name).Result;

			if (currentEmployee.RestaurantId.HasValue)
				return !currentEmployee.Restaurant.Tables
					.All(t => t.Reservations
						.All(r => r.Id == reservation.Id || !(r.TableId == reservation.TableId && reservation.StartTime < r.EndTime && reservation.EndTime > r.StartTime)));
			else
				return !currentEmployee.OwnedRestaurants
					.All(r => r.Tables
						.All(t => t.Reservations
							.All(re => re.Id == reservation.Id || !(re.TableId == reservation.TableId && reservation.StartTime < re.EndTime && reservation.EndTime > re.StartTime))));
		}

		private bool IsUserAllowedToAccessReservation(Reservation reservation)
		{
			var currentEmployee = _repo.GetEmployeeByName(User.Identity.Name).Result;
			return (currentEmployee.RestaurantId.HasValue && currentEmployee.Restaurant.Tables.Any(t => t.Id == reservation.TableId)) ||
				(!currentEmployee.RestaurantId.HasValue && currentEmployee.OwnedRestaurants.Any(r => r.Tables.Any(t => t.Id == reservation.TableId)));
		}
	}
}
