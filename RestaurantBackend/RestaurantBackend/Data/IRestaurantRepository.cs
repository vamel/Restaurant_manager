using RestaurantBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantBackend.Data
{
	public interface IRestaurantRepository
	{
		Task<T> Add<T>(T entity) where T : class;
		T Update<T>(T entity) where T : class;
		void Delete<T>(T entity) where T : class;
		Task<Restaurant> GetRestaurant(int id);
		IEnumerable<Restaurant> GetAllRestaurants();
		IEnumerable<Restaurant> GetOwnedRestaurants(int ownerId);
		Task<Employee> GetEmployee(int id);
		Task<bool> IsEmployeeInRestaurant(int employeeId, int restaurantId);
		Task<Employee> GetEmployeeByName(string name);
		IEnumerable<Employee> GetRestaurantEmployees(int restaurantId);
		Task<string> GetEmployeeRole(int employeeId);
		IEnumerable<string> GetEmployeeRoles(int employeeId);
		Task<Employee> GetRestaurantOwner(int restaurantId);
		IEnumerable<Employee> GetRestaurantManagers(int restaurantId);
		Task<RestaurantIngredient> GetRestaurantIngredient(int restaurantId, int ingredientId);
		IEnumerable<RestaurantIngredient> GetRestaurantIngredientsByRestaurant(int restaurantId);
		IEnumerable<RestaurantIngredient> GetRestaurantIngredientsByIngredient(int ingredientId);
		Task<Address> GetAddress(int id);
		Task<Table> GetTable(int id);
		Task<Reservation> GetReservation(int id);
		IEnumerable<Table> GetRestaurantTables(int id);
		IEnumerable<Reservation> GetRestaurantReservations(int restaurantId);
		IEnumerable<Reservation> GetTableReservations(int tableId);
		Task<Country> GetCountry(int id);
		IEnumerable<Address> GetAllAddresses();
		IEnumerable<Country> GetAllCountries();
		IEnumerable<Menu> GetAllMenus();
		Task<Menu> GetMenu(int id);
		IEnumerable<Dish> GetMenuDishes(int id);
		Task<Dish> GetDish(int id);
		IEnumerable<DishIngredient> GetDishIngredients(int id);
		Task<Ingredient> GetIngredient(int id);
		IEnumerable<Ingredient> GetIngredients();
		Task<Supplier> GetSupplier(int id);
		Task<SupplierIngredient> GetSupplierIngredient(int supplierId, int ingredientId);
		IEnumerable<SupplierIngredient> GetSupplierIngredients(int supplierId);
		Task<SupplierOrder> GetSupplierOrder(int id);
		IEnumerable<SupplierOrder> GetSupplierOrders(int supplierId);
		IEnumerable<OrderIngredient> GetSupplierOrderIngredients(int orderId);
		IEnumerable<SupplierOrder> GetRestaurantSupplierOrders(int restaurantId, int supplierId);
		IEnumerable<SupplierOrder> GetRestaurantSupplierOrders(int restaurantId);
		IEnumerable<Supplier> GetSuppliers();
		Task<CustomerOrder> GetCustomerOrder(int id);
		Task<Dish> GetCustomerOrderDish(int orderId, int dishId);
		IEnumerable<CustomerOrder> GetCustomerOrders(int restaurantId);
	}
}
