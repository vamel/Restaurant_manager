using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Data.DTOs
{
    public class RegisterOwnerDto
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surame is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
		public string PESEL { get; set; }
        [Required(ErrorMessage = "Birth date is required")]
		public DateTime BirthDate { get; set; }

	}
}
