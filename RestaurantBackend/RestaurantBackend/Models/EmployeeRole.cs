using Microsoft.AspNetCore.Identity;

namespace RestaurantBackend.Models
{
	public class EmployeeRole : IdentityUserRole<int>
	{
		public virtual Employee User { get; set; }
		public virtual ApplicationRole Role { get; set; }
	}
}
