using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class TableForCreateDto
	{
		public int SeatCount { get; set; }
		public string Name { get; set; }
		public int RestaurantId { get; set; }

		public Table ToTable()
		{
			return new Table { SeatCount = SeatCount, Name = Name, RestaurantId = RestaurantId };
		}
	}
}
