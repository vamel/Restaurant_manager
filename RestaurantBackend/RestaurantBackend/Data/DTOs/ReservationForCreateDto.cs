using RestaurantBackend.Models;
using System;

namespace RestaurantBackend.Data.DTOs
{
	public class ReservationForCreateDto
	{
		public int TableId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Name { get; set; }

		public Reservation ToReservation()
		{
			return new Reservation { TableId = TableId, StartTime = StartTime, EndTime = EndTime, Name = Name };
		}
	}
}
