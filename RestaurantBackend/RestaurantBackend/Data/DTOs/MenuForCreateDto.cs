using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class MenuForCreateDto
	{
		public IEnumerable<DishDto> Dishes { get; set; }
	}
}