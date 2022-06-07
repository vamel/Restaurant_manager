using RestaurantBackend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Data.DTOs
{
    public class RestaurantForCreateDto
	{
		[Required(ErrorMessage = "Name required")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Address required")]
		public AddressForCreateDto Address { get; set; }
		[Required(ErrorMessage = "Owner ID required")]
		public int OwnerId { get; set; }
		public int? ExistingMenuId { get; set; }
	}
}
