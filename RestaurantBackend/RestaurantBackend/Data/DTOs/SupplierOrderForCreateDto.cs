using RestaurantBackend.Models;
using System;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class SupplierOrderForCreateDto
	{
		public int SupplierId { get; set; }
		public int RestaurantId { get; set; }
		public IEnumerable<OrderIngredientForCreateDto> OrderIngredients { get; set; }

		public SupplierOrder ToSupplierOrder()
		{
			return new SupplierOrder
			{
				SupplierId = SupplierId,
				RestaurantId = RestaurantId,
				Date = DateTime.Now,
			};
		}
	}
}
