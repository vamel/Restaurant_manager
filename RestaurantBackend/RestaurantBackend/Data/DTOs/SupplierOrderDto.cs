using RestaurantBackend.Models;
using System;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class SupplierOrderDto
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public int SupplierId { get; set; }
		public int RestaurantId { get; set; }
		public IEnumerable<OrderIngredientDto> OrderIngredients { get; set; }

		public SupplierOrderDto() { }

		public SupplierOrderDto(SupplierOrder order)
		{
			Id = order.Id;
			Date = order.Date;
			SupplierId = order.SupplierId;
			RestaurantId = order.RestaurantId;

			var l = new List<OrderIngredientDto>();

			if (order.OrderIngredients != null)
			{
				foreach (var item in order.OrderIngredients)
				{
					l.Add(new OrderIngredientDto(item));
				}
			}

			OrderIngredients = l;
		}
	}
}
