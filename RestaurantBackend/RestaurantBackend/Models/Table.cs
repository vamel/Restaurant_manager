using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class Table
	{
		public int Id { get; set; }
		public int SeatCount { get; set; }
		public string Name { get; set; }
		public int RestaurantId { get; set; }
		public Restaurant Restaurant { get; set; }
		public ICollection<Reservation> Reservations { get; set; }
	}
}
