using RestaurantBackend.Models;

namespace RestaurantBackend.Data.DTOs
{
	public class RoleUpdateDto
	{
		public int EmployeeId { get; set; }
		public string NewRole { get; set; }
	}
}