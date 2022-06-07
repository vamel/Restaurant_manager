using RestaurantBackend.Models;
using System;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class CustomerOrderDto
	{
		public int Id { get; set; }
		public bool Takeout { get; set; }
		public DateTime OrderTime { get; set; }
		public int TotalPrice { get; set; }
		public int AssignedEmployeeId { get; set; }
		public int RestaurantId { get; set; }
		public IEnumerable<OrderDishDto> Dishes { get; set; }

		public CustomerOrderDto() {	}
		public CustomerOrderDto(CustomerOrder customerOrder)
		{ 
			Id = customerOrder.Id;
			Takeout = customerOrder.Takeout;
			OrderTime = customerOrder.OrderTime;
			TotalPrice = customerOrder.TotalPrice;
			AssignedEmployeeId = customerOrder.AssignedEmployeeId;
			RestaurantId = customerOrder.RestaurantId;

			var l = new List<OrderDishDto>();

			if (customerOrder.Dishes != null)
			{
				foreach (var item in customerOrder.Dishes)
					l.Add(new OrderDishDto(item));
			}

			Dishes = l;
		}
	}
}
