using RestaurantBackend.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantBackend.Data.DTOs
{
	public class SupplierForCreateDto
	{
		[Required(ErrorMessage = "Name required.")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Bank information required.")]
		public string BankInformation { get; set; }
		public int? AddressId { get; set; }
		public AddressForCreateDto Address { get; set; }
	}
}
