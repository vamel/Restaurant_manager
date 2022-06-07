using Microsoft.AspNetCore.Identity;
using RestaurantBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Data.DTOs
{
	public class EmployeeForUpdateAsManagerDto
    {
		public int Id { get; set; }
		public int Salary { get; set; }
	}
}
