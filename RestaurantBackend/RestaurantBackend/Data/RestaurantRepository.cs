using Microsoft.EntityFrameworkCore;
using RestaurantBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantBackend.Data
{
	public class RestaurantRepository : IRestaurantRepository
	{
		private readonly ApplicationDbContext _context;
		public RestaurantRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<T> Add<T>(T entity) where T : class
		{
			var newEntity = (await _context.AddAsync<T>(entity)).Entity;

			await _context.SaveChangesAsync();

			return newEntity;
		}

		public T Update<T>(T entity) where T : class
		{
			var newEntity = _context.Update(entity).Entity;

			_context.SaveChanges();

			return newEntity;
		}

		public async void Delete<T>(T entity) where T : class
		{
			_context.Remove(entity);

			await _context.SaveChangesAsync();
		}

		public async Task<Address> GetAddress(int id)
		{
			return await _context.Addresses
				.Include(a => a.Country)
				.Include(a => a.Restaurant)
				.Include(a => a.Suppliers)
				.FirstOrDefaultAsync(a => a.Id == id);
		}

		public IEnumerable<Address> GetAllAddresses()
		{
			return _context.Addresses
				.Include(a => a.Country)
				.Include(a => a.Restaurant)
				.Include(a => a.Suppliers).
				AsEnumerable();
		}

		public IEnumerable<Country> GetAllCountries()
		{
			return _context.Countries
				.Include(c => c.Addresses)
				.AsEnumerable();
		}

		public IEnumerable<Menu> GetAllMenus()
		{
			return _context.Menus.AsEnumerable();
		}

		public IEnumerable<Restaurant> GetAllRestaurants()
		{
			return _context.Restaurants
				.Include(r => r.Owner)
				.Include(r => r.Address)
					.ThenInclude(a => a.Country)
				.Include(r => r.Menu)
				.Include(r => r.Menu.Dishes)
				.Include(r => r.Tables)
					.ThenInclude(t => t.Reservations)
				.AsEnumerable();
		}

		public async Task<Country> GetCountry(int id)
		{
			return await _context.Countries
				.Include(a => a.Addresses)
				.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<Employee> GetEmployee(int id)
		{
			return await _context.Users
				.Include(u => u.Restaurant)
				.Include(u => u.Restaurant.Address)
					.ThenInclude(a => a.Country)
				.Include(u => u.Restaurant.Menu)
				.Include(u => u.Restaurant.Menu.Dishes)
				.Include(u => u.Restaurant.Tables)
					.ThenInclude(t => t.Reservations)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Address)
						.ThenInclude(a => a.Country)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Menu)
						.ThenInclude(m => m.Dishes)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Ingredients)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Tables)
						.ThenInclude(t => t.Reservations)
				.Include(u => u.CustomerOrders)
				.AsSplitQuery()
				.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<Employee> GetEmployeeByName(string name)
		{
			return await _context.Users
				.Include(u => u.Restaurant)
				.Include(u => u.Restaurant.Address)
					.ThenInclude(a => a.Country)
				.Include(u => u.Restaurant.Menu)
				.Include(u => u.Restaurant.Menu.Dishes)
				.Include(u => u.Restaurant.Tables)
					.ThenInclude(t => t.Reservations)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Address)
						.ThenInclude(a => a.Country)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Ingredients)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Ingredients)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Menu)
						.ThenInclude(m => m.Dishes)
				.Include(u => u.OwnedRestaurants)
					.ThenInclude(r => r.Tables)
						.ThenInclude(t => t.Reservations)
				.Include(u => u.CustomerOrders)
				.AsSplitQuery()
				.FirstOrDefaultAsync(c => c.UserName == name);
		}

		public async Task<string> GetEmployeeRole(int employeeId)
		{
			return (await _context.UserRoles
				.Include(ur => ur.User)
				.Include(ur => ur.Role)
				.FirstOrDefaultAsync(ur => ur.UserId == employeeId))?.Role.Name;
		}

		public IEnumerable<string> GetEmployeeRoles(int employeeId)
		{
			return _context.UserRoles
				.Include(ur => ur.User)
				.Include(ur => ur.Role)
				.Where(ur => ur.UserId == employeeId)
				.Select(ur => ur.Role.Name);
		}

		public async Task<Menu> GetMenu(int id)
		{
			return await _context.Menus
				.Include(m => m.Dishes)
				.FirstOrDefaultAsync(c => c.Id == id);
		}

		public IEnumerable<Dish> GetMenuDishes(int id)
		{
			return _context.Dishes
				.Include(d => d.DishIngredients)
				.Include(d => d.Orders)
				.Include(d => d.Menu)
				.Where(d => d.MenuId == id);
		}

		public IEnumerable<Restaurant> GetOwnedRestaurants(int ownerId)
		{
			return _context.Restaurants
				.Include(r => r.Owner)
				.Include(r => r.Address)
					.ThenInclude(a => a.Country)
				.Include(r => r.Menu)
				.Include(r => r.Menu.Dishes)
					.ThenInclude(d => d.DishIngredients)
				.Include(r => r.Ingredients)
				.Include(r => r.Tables)
					.ThenInclude(t => t.Reservations)
				.AsSplitQuery()
				.Where(r => r.OwnerId == ownerId);
		}

		public async Task<Restaurant> GetRestaurant(int id)
		{
			return await _context.Restaurants
				.Include(r => r.Owner)
				.Include(r => r.Address)
					.ThenInclude(a => a.Country)
				.Include(r => r.Menu)
				.Include(r => r.Menu.Dishes)
					.ThenInclude(d => d.DishIngredients)
				.Include(r => r.Ingredients)
				.Include(r => r.Tables)
					.ThenInclude(t => t.Reservations)
				.AsSplitQuery()
				.FirstOrDefaultAsync(c => c.Id == id);
		}

		public IEnumerable<Employee> GetRestaurantEmployees(int restaurantId)
		{
			return _context.Users
				.Include(u => u.Restaurant)
				.Include(u => u.OwnedRestaurants)
				.Include(u => u.CustomerOrders)
				.Where(u => u.RestaurantId == restaurantId)
				.AsEnumerable();
		}

		public async Task<RestaurantIngredient> GetRestaurantIngredient(int restaurantId, int ingredientId)
		{
			return await _context.RestaurantIngredients
				.Include(ri => ri.Restaurant)
				.Include(ri => ri.Ingredient)
				.FirstOrDefaultAsync(ri => ri.RestaurantId == restaurantId && ri.IngredientId == ingredientId);
		}

		public IEnumerable<RestaurantIngredient> GetRestaurantIngredientsByIngredient(int ingredientId)
		{
			return _context.RestaurantIngredients
				.Include(ri => ri.Restaurant)
				.Include(ri => ri.Ingredient)
				.Where(ri => ri.IngredientId == ingredientId)
				.AsEnumerable();
		}

		public IEnumerable<RestaurantIngredient> GetRestaurantIngredientsByRestaurant(int restaurantId)
		{
			return _context.RestaurantIngredients
				.Include(ri => ri.Restaurant)
				.Include(ri => ri.Ingredient)
				.Where(ri => ri.RestaurantId == restaurantId)
				.AsEnumerable();
		}

		public IEnumerable<Employee> GetRestaurantManagers(int restaurantId)
		{
			throw new System.NotImplementedException();
		}

		public Task<Employee> GetRestaurantOwner(int restaurantId)
		{
			throw new System.NotImplementedException();
		}

		public async Task<bool> IsEmployeeInRestaurant(int employeeId, int restaurantId)
		{
			var employee = await GetEmployee(employeeId);
			var restaurant = await GetRestaurant(restaurantId);
			if (employee == null || restaurant == null) return false;

			return (!employee.RestaurantId.HasValue && employee.Id == restaurant.OwnerId) || (employee.RestaurantId.HasValue && employee.RestaurantId == restaurantId);
		}

		public IEnumerable<DishIngredient> GetDishIngredients(int id)
		{
			return _context.DishIngredients
				.Include(di => di.Dish)
				.Include(di => di.Ingredient)
				.Where(di => di.DishId == id)
				.AsEnumerable();
		}

		public Task<Dish> GetDish(int id)
		{
			return _context.Dishes
				.Include(d => d.DishIngredients)
				.Include(d => d.Menu)
				.Include(d => d.Orders)
				.FirstOrDefaultAsync(d => d.Id == id);
		}

		public async Task<Ingredient> GetIngredient(int id)
		{
			return await _context.Ingredients
				.Include(i => i.RestaurantIngredients)
				.Include(i => i.SupplierOrders)
				.Include(i => i.DishIngredients)
				.Include(i => i.SupplierIngredients)
				.FirstOrDefaultAsync(i => i.Id == id);
		}

		public IEnumerable<Ingredient> GetIngredients()
		{
			return _context.Ingredients
				.Include(i => i.RestaurantIngredients)
				.Include(i => i.SupplierOrders)
				.Include(i => i.DishIngredients)
				.Include(i => i.SupplierIngredients)
				.AsEnumerable();
		}

		public Task<Table> GetTable(int id)
		{
			return _context.Tables
				.Include(t => t.Restaurant)
				.Include(t => t.Reservations)
				.FirstOrDefaultAsync(t => t.Id == id);
		}

		public Task<Reservation> GetReservation(int id)
		{
			return _context.Reservations
				.Include(r => r.Table)
				.FirstOrDefaultAsync(r => r.Id == id);
		}

		public IEnumerable<Table> GetRestaurantTables(int id)
		{
			return _context.Tables
				.Include(t => t.Restaurant)
				.Include(t => t.Reservations)
				.Where(t => t.RestaurantId == id);
		}

		public IEnumerable<Reservation> GetRestaurantReservations(int restaurantId)
		{
			return _context.Reservations
				.Include(r => r.Table)
				.Where(r => r.Table.RestaurantId == restaurantId);
		}

		public IEnumerable<Reservation> GetTableReservations(int tableId)
		{
			return _context.Reservations
				.Include(r => r.Table)
				.Where(r => r.TableId == tableId);
		}

		public async Task<Supplier> GetSupplier(int id)
		{
			return await _context.Suppliers
				.Include(s => s.Ingredients)
				.Include(s => s.Address)
					.ThenInclude(a => a.Country)
				.FirstOrDefaultAsync(s => s.Id == id);
		}

		public IEnumerable<Supplier> GetSuppliers()
		{
			return _context.Suppliers
				.Include(s => s.Ingredients)
				.Include(s => s.Address)
					.ThenInclude(a => a.Country)
				.AsEnumerable();
		}

		public async Task<SupplierOrder> GetSupplierOrder(int id)
		{
			return await _context.SupplierOrders
				.Include(so => so.Restaurant)
				.Include(so => so.Supplier)
				.Include(so => so.OrderIngredients)
				.FirstOrDefaultAsync(so => so.Id == id);
		}

		public IEnumerable<SupplierOrder> GetSupplierOrders(int supplierId)
		{
			return _context.SupplierOrders
				.Include(so => so.Restaurant)
				.Include(so => so.Supplier)
				.Include(so => so.OrderIngredients)
				.Where(so => so.SupplierId == supplierId);
		}

		public IEnumerable<OrderIngredient> GetSupplierOrderIngredients(int orderId)
		{
			return _context.OrderIngredients
				.Include(oi => oi.SupplierOrder)
				.Include(oi => oi.Ingredient)
				.Where(oi => oi.SupplierOrderId == orderId);
		}

		public async Task<OrderIngredient> GetSupplierOrderIngredient(int orderId, int ingredientId)
		{
			return await _context.OrderIngredients
				.Include(oi => oi.SupplierOrder)
				.Include(oi => oi.Ingredient)
				.FirstOrDefaultAsync(oi => oi.SupplierOrderId == orderId && oi.IngredientId == ingredientId);
		}

		public IEnumerable<SupplierOrder> GetRestaurantSupplierOrders(int restaurantId, int supplierId)
		{
			return _context.SupplierOrders
				.Include(so => so.Restaurant)
				.Include(so => so.Supplier)
				.Include(so => so.OrderIngredients)
				.Where(so => so.RestaurantId == restaurantId && so.SupplierId == supplierId);
		}

		public IEnumerable<SupplierOrder> GetRestaurantSupplierOrders(int restaurantId)
		{
			return _context.SupplierOrders
				.Include(so => so.Restaurant)
				.Include(so => so.Supplier)
				.Include(so => so.OrderIngredients)
				.Where(so => so.RestaurantId == restaurantId);
		}

		public async Task<SupplierIngredient> GetSupplierIngredient(int supplierId, int ingredientId)
		{
			return await _context.SupplierIngredients
				.Include(si => si.Ingredient)
				.Include(si => si.Supplier)
				.FirstOrDefaultAsync(si => si.IngredientId == ingredientId && si.SupplierId == supplierId);
		}

		public IEnumerable<SupplierIngredient> GetSupplierIngredients(int supplierId)
		{
			return _context.SupplierIngredients
				.Include(si => si.Ingredient)
				.Include(si => si.Supplier)
				.Where(si => si.SupplierId == supplierId);
		}

		public async Task<CustomerOrder> GetCustomerOrder(int id)
		{
			return await _context.CustomerOrders
				.Include(co => co.Restaurant)
				.Include(co => co.AssignedEmployee)
				.Include(co => co.Dishes)
					.ThenInclude(od => od.Dish)
						.ThenInclude(d => d.DishIngredients)
				.FirstOrDefaultAsync(co => co.Id == id);
		}

		public Task<Dish> GetCustomerOrderDish(int orderId, int dishId)
		{
			return _context.Dishes
				.Include(d => d.DishIngredients)
				.Include(d => d.Orders)
				.Include(d => d.Menu)
				.FirstOrDefaultAsync(d => d.Id == dishId && d.Orders.Any(o => o.OrderId == orderId));
		}

		public IEnumerable<CustomerOrder> GetCustomerOrders(int restaurantId)
		{
			return _context.CustomerOrders
				.Include(co => co.Restaurant)
				.Include(co => co.AssignedEmployee)
				.Include(co => co.Dishes)
					.ThenInclude(od => od.Dish)
						.ThenInclude(d => d.DishIngredients)
				.Where(co => co.RestaurantId == restaurantId);
		}
	}
}
