using RestaurantBackend.Models;
using System;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class CustomerOrderForCreateDto
	{
		public bool Takeout { get; set; }
		public DateTime OrderTime { get; set; }
		public int TotalPrice { get; set; }
		public int AssignedEmployeeId { get; set; }
		public int RestaurantId { get; set; }
		public IEnumerable<OrderDishForCreateDto> Dishes { get; set; }

		public CustomerOrder ToCustomerOrder()
		{
			return new CustomerOrder
			{
				Takeout = Takeout,
				OrderTime = OrderTime,
				TotalPrice = TotalPrice,
				AssignedEmployeeId = AssignedEmployeeId,
				RestaurantId = RestaurantId
			};
		}
	}
}
