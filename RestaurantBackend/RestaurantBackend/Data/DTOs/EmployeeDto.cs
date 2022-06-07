using Microsoft.AspNetCore.Identity;
using RestaurantBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Data.DTOs
{
	public class EmployeeDto
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public int Salary { get; set; }
		public DateTime BirthDate { get; set; }
		public string PESEL { get; set; }
		public int RestaurantId { get; set; } // -1 if null

		public EmployeeDto() { }

		public EmployeeDto(Employee user)
		{
			Id = user.Id;
			Name = user.Name;
			Surname = user.Surname;
			PESEL = user.PESEL;
			RestaurantId = user.RestaurantId ?? -1;
			BirthDate = user.BirthDate;
			Salary = user.Salary;
		}
	}
}
