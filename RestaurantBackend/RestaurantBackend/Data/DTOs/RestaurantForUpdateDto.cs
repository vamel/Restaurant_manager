using RestaurantBackend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Data.DTOs
{
    public class RestaurantForUpdateDto
	{
		[Required(ErrorMessage = "Id required")]
		public int Id { get; set; }
		[Required(ErrorMessage = "Name required")]
		public string Name { get; set; }
	}
}
