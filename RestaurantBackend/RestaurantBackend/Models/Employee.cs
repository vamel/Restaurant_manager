using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Models
{
	public class Employee : IdentityUser<int>
    {
		public ICollection<EmployeeRole> UserRoles { get; set; }

		[ProtectedPersonalData]
		public string Name { get; set; }
		[ProtectedPersonalData]
		public string Surname { get; set; }
		public int Salary { get; set; }
		public DateTime BirthDate { get; set; }
		public string PESEL { get; set; }
		public ICollection<CustomerOrder> CustomerOrders { get; set; }
		public int? RestaurantId { get; set; }
		public Restaurant Restaurant { get; set; }
		public ICollection<Restaurant> OwnedRestaurants { get; set; }
	}
}
