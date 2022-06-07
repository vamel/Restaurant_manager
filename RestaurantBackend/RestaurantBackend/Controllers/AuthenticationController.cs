using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RestaurantBackend.Models;
using RestaurantBackend.Data;
using RestaurantBackend.Data.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantBackend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthenticationController : ControllerBase
	{
        private readonly UserManager<Employee> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly IRestaurantRepository _repo;
        public AuthenticationController(UserManager<Employee> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IRestaurantRepository repo)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _repo = repo;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await userManager.FindByNameAsync(model.Username);

            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    employeeId = user.Id
                });
            }
            return Unauthorized();
        }

        [Authorize(Roles=EmployeeRoles.Owner)]
        [HttpPost]
        [Route("registerEmployee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            var currentUser = await _repo.GetEmployeeByName(User.Identity.Name);

            if (!currentUser.OwnedRestaurants.Any(r => r.Id == model.RestaurantId))
                return BadRequest(new Response
                {
                    Status = "Bad Request",
                    Message = "Cannot add employee to a restaurant you don't own."
                });


            Employee user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name,
                Surname = model.Surname,
                BirthDate = model.BirthDate,
                PESEL = model.PESEL,
                RestaurantId = model.RestaurantId,
            };


            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = result.Errors.Aggregate<IdentityError, string>("", (a, b) => {
                        a += $"{b.Code}: {b.Description}\n";
                        return a;
                    })
                });

            if (await roleManager.RoleExistsAsync(EmployeeRoles.Employee))
            {
                await userManager.AddToRoleAsync(user, EmployeeRoles.Employee);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("registerOwner")]
        public async Task<IActionResult> RegisterOwner([FromBody] RegisterOwnerDto model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            Employee user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name,
                Surname = model.Surname,
                BirthDate = model.BirthDate,
                PESEL = model.PESEL,
                RestaurantId = null
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = result.Errors.Aggregate<IdentityError, string>("", (a, b) => {
                        a += $"{b.Code}: {b.Description}\n";
                        return a;
                    })
                });
            }

            if (!await roleManager.RoleExistsAsync(EmployeeRoles.Owner))
                await roleManager.CreateAsync(new ApplicationRole() { Name = EmployeeRoles.Owner, NormalizedName = EmployeeRoles.Owner.ToUpper() });
            if (!await roleManager.RoleExistsAsync(EmployeeRoles.Manager))
                await roleManager.CreateAsync(new ApplicationRole() { Name = EmployeeRoles.Manager, NormalizedName = EmployeeRoles.Manager.ToUpper() }); ;
            if (!await roleManager.RoleExistsAsync(EmployeeRoles.Employee))
                await roleManager.CreateAsync(new ApplicationRole() { Name = EmployeeRoles.Employee, NormalizedName = EmployeeRoles.Employee.ToUpper() });

            if (await roleManager.RoleExistsAsync(EmployeeRoles.Owner))
            {
                await userManager.AddToRoleAsync(user, EmployeeRoles.Owner);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize(Roles = EmployeeRoles.Owner)]
        [HttpPost]
        [Route("updateEmployeeRole")]
        public async Task<IActionResult> UpdateEmployeeRole([FromBody] RoleUpdateDto roleUpdate)
        {
            var employee = await userManager.FindByIdAsync(roleUpdate.EmployeeId.ToString());
            if (employee == null) return NotFound();

            var empRoles = _repo.GetEmployeeRoles(employee.Id);


            if (empRoles.Any(ur => ur == EmployeeRoles.Owner))
                return BadRequest(new Response { Status = "Bad request", Message = "Owners stay owners." });


            var currentUser = (await _repo.GetEmployeeByName(User.Identity.Name)).Id;
            if (employee.RestaurantId.HasValue && !(await _repo.IsEmployeeInRestaurant(currentUser, employee.RestaurantId.Value)))
                return Unauthorized(new Response() { Status = "Unauthorized", Message = "Different restaurant!" });

            if (!await roleManager.RoleExistsAsync(roleUpdate.NewRole))
                return BadRequest(new Response() { Status = "Bad request", Message = "Role does not exist." });

            foreach (var role in empRoles)
                await userManager.RemoveFromRoleAsync(employee, role);

            await userManager.AddToRoleAsync(employee, roleUpdate.NewRole);

            return Ok(new Response { Status = "Success", Message = "Role changed successfully."});
        }

        // This was only needed once but keeping it here for emergencies  (total database failure)

        #region ADMIN
        /*[HttpPost]
        [Route("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterOwnerDto model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            Employee user = new Employee()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name,
                Surname = model.Surname,
                BirthDate = model.BirthDate,
                PESEL = model.PESEL,
                RestaurantId = null
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = result.Errors.Aggregate<IdentityError, string>("", (a, b) => {
                        a += $"{b.Code}: {b.Description}\n";
                        return a;
                    })
                });
            }


            if (!await roleManager.RoleExistsAsync(EmployeeRoles.Admin))
                await roleManager.CreateAsync(new ApplicationRole() { Name = EmployeeRoles.Admin, NormalizedName = EmployeeRoles.Admin.ToUpper() });
            if (!await roleManager.RoleExistsAsync(EmployeeRoles.Owner))
                await roleManager.CreateAsync(new ApplicationRole() { Name = EmployeeRoles.Owner, NormalizedName = EmployeeRoles.Owner.ToUpper() });
            if (!await roleManager.RoleExistsAsync(EmployeeRoles.Manager))
                await roleManager.CreateAsync(new ApplicationRole() { Name = EmployeeRoles.Manager, NormalizedName = EmployeeRoles.Manager.ToUpper() }); ;
            if (!await roleManager.RoleExistsAsync(EmployeeRoles.Employee))
                await roleManager.CreateAsync(new ApplicationRole() { Name = EmployeeRoles.Employee, NormalizedName = EmployeeRoles.Employee.ToUpper() });

            if (await roleManager.RoleExistsAsync(EmployeeRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, EmployeeRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }*/
        #endregion
    }
}
