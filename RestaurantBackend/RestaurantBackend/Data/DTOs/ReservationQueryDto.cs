using RestaurantBackend.Models;
using System;

namespace RestaurantBackend.Data.DTOs
{
	public class ReservationQueryDto
	{
		public int RestaurantId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}
