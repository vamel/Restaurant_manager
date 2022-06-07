using System;

namespace RestaurantBackend.Models
{
	public class Reservation
	{
		public int Id { get; set; }
		public int TableId { get; set; }
		public Table Table { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Name { get; set; }
	}
}
