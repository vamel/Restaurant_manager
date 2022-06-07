using RestaurantBackend.Models;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class TableDto
	{
		public int Id { get; set; }
		public int SeatCount { get; set; }
		public string Name { get; set; }
		public int RestaurantId { get; set; }
		public IEnumerable<ReservationDto> Reservations { get; set; }

		public TableDto() { }

		public TableDto(Table table)
		{
			Id = table.Id;
			SeatCount = table.SeatCount;
			Name = table.Name;
			RestaurantId = table.RestaurantId;

			var l = new List<ReservationDto>();

			if (table.Reservations != null)
			{
				foreach (var item in table.Reservations)
				{
					l.Add(new ReservationDto(item));
				}
			}

			Reservations = l;
		}
	}
}
