using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace RestaurantBackend.Models
{
	public class ApplicationRole : IdentityRole<int>
	{
		public virtual ICollection<EmployeeRole> UserRoles { get; set; }
	}
}
