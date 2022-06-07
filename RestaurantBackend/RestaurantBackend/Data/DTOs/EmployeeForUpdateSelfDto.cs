using Microsoft.AspNetCore.Identity;
using RestaurantBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Data.DTOs
{
	public class EmployeeForUpdateSelfDto
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public DateTime BirthDate { get; set; }
		public string PESEL { get; set; }
	}
}
