using RestaurantBackend.Models;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class SupplierForUpdateDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string BankInformation { get; set; }
	}
}
