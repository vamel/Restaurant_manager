using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class OrderDishForCreateDto
	{
		public int DishId { get; set; }
		public int Count { get; set; }

		public OrderDish ToOrderDish()
		{
			return new OrderDish
			{
				DishId = DishId,
				Count = Count
			};
		}
	}
}
