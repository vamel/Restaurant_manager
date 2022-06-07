using System;
using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class CustomerOrder
	{
		public int Id { get; set; }
		public bool Takeout { get; set; }
		public DateTime OrderTime { get; set; }
		public int TotalPrice { get; set; }
		public int AssignedEmployeeId { get; set; }
		public int RestaurantId { get; set; }
		public Restaurant Restaurant { get; set; }
		public Employee AssignedEmployee { get; set; }
		public ICollection<OrderDish> Dishes { get; set; }
	}
}
