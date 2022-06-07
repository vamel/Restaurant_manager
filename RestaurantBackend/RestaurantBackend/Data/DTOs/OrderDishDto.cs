using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class OrderDishDto
	{
		public int OrderId { get; set; }
		public int DishId { get; set; }
		public int Count { get; set; }

		public OrderDishDto() { }
		public OrderDishDto(OrderDish orderDish)
		{
			OrderId = orderDish.OrderId;
			DishId = orderDish.DishId;
			Count = orderDish.Count;
		}
	}
}
