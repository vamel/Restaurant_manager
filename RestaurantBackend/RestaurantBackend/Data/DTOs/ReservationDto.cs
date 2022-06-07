using RestaurantBackend.Models;
using System;

namespace RestaurantBackend.Data.DTOs
{
	public class ReservationDto
	{
		public int Id { get; set; }
		public int TableId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Name { get; set; }

		public ReservationDto() { }
		public ReservationDto(Reservation reservation)
		{
			Id = reservation.Id;
			TableId = reservation.TableId;
			StartTime = reservation.StartTime;
			EndTime = reservation.EndTime;
			Name = reservation.Name;
		}
	}
}
