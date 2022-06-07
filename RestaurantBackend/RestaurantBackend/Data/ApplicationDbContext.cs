using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantBackend.Models;

namespace RestaurantBackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee, 
                                                          ApplicationRole, 
                                                          int, 
                                                          IdentityUserClaim<int>, 
                                                          EmployeeRole, 
                                                          IdentityUserLogin<int>, 
                                                          IdentityRoleClaim<int>, 
                                                          IdentityUserToken<int>>
	{
		public DbSet<Address> Addresses { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<CustomerOrder> CustomerOrders { get; set; }
		public DbSet<Dish> Dishes { get; set; }
		public DbSet<DishIngredient> DishIngredients { get; set; }
		public DbSet<Ingredient> Ingredients { get; set; }
		public DbSet<Menu> Menus { get; set; }
		public DbSet<OrderDish> OrderDishes { get; set; }
		public DbSet<OrderIngredient> OrderIngredients { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<Restaurant> Restaurants { get; set; }
		public DbSet<RestaurantIngredient> RestaurantIngredients { get; set; }
		public DbSet<Supplier> Suppliers { get; set; }
		public DbSet<SupplierIngredient> SupplierIngredients { get; set; }
		public DbSet<SupplierOrder> SupplierOrders { get; set; }
		public DbSet<Table> Tables { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected void IdentityAdjustments(ModelBuilder builder)
		{
            builder.Entity<Employee>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();
            });

            builder.Entity<Employee>().ToTable("Users").HasKey(x => x.Id);

            builder.Entity<ApplicationRole>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            builder.Entity<ApplicationRole>().ToTable("Roles").HasKey(x => x.Id);

            builder.Entity<EmployeeRole>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins").HasKey(x => x.UserId);
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims").HasKey(x => x.Id);
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims").HasKey(x => x.Id);
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens").HasKey(x => x.UserId);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // setting up the identity stuff

            IdentityAdjustments(builder);

            // everything else

            builder.Entity<Address>(b =>
			{
                b.HasOne(x => x.Country)
                    .WithMany(x => x.Addresses)
                    .HasForeignKey(x => x.CountryId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasKey(x => x.Id);
            });
            
            builder.Entity<Country>().HasKey(x => x.Id);
            
            builder.Entity<CustomerOrder>(b =>
			{
                b.HasOne(x => x.AssignedEmployee)
                    .WithMany(x => x.CustomerOrders)
                    .HasForeignKey(x => x.AssignedEmployeeId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Restaurant)
                    .WithMany(x => x.CustomerOrders)
                    .HasForeignKey(x => x.RestaurantId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

				b.HasKey(x => x.Id);
			});

            builder.Entity<Dish>(b =>
            {
                b.HasOne(x => x.Menu)
                    .WithMany(x => x.Dishes)
                    .HasForeignKey(x => x.MenuId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasKey(x => x.Id);
            });

            builder.Entity<DishIngredient>(b =>
            {
                b.HasKey(x => new { x.DishId, x.IngredientId });

                b.HasOne(x => x.Dish)
                    .WithMany(x => x.DishIngredients)
                    .HasForeignKey(x => x.DishId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasOne(x => x.Ingredient)
                    .WithMany(x => x.DishIngredients)
                    .HasForeignKey(x => x.IngredientId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            builder.Entity<Employee>(b =>
            {
                b.HasOne(x => x.Restaurant)
                    .WithMany(x => x.Employees)
                    .HasForeignKey(x => x.RestaurantId)
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasKey(x => x.Id);
            });

            builder.Entity<Ingredient>().HasKey(x => x.Id);

            builder.Entity<Menu>().HasKey(x => x.Id);

            builder.Entity<OrderDish>(b =>
            {
                b.HasKey(x => new { x.OrderId, x.DishId });

                b.HasOne(x => x.Dish)
                    .WithMany(x => x.Orders)
                    .HasForeignKey(x => x.DishId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasOne(x => x.Order)
                    .WithMany(x => x.Dishes)
                    .HasForeignKey(x => x.OrderId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            builder.Entity<OrderIngredient>(b =>
            {
                b.HasKey(x => new { x.SupplierOrderId, x.IngredientId });

                b.HasOne(x => x.SupplierOrder)
                    .WithMany(x => x.OrderIngredients)
                    .HasForeignKey(x => x.SupplierOrderId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasOne(x => x.Ingredient)
                    .WithMany(x => x.SupplierOrders)
                    .HasForeignKey(x => x.IngredientId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            builder.Entity<Reservation>(b =>
            {
                b.HasOne(x => x.Table)
                    .WithMany(x => x.Reservations)
                    .HasForeignKey(x => x.TableId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasKey(x => x.Id);
            });

            builder.Entity<Restaurant>(b =>
            {
                b.HasOne(x => x.Owner)
                    .WithMany(x => x.OwnedRestaurants)
                    .HasForeignKey(x => x.OwnerId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasOne(x => x.Address).WithMany(x => x.Restaurant).HasForeignKey(x => x.AddressId);

                b.HasKey(x => x.Id);
            });

            builder.Entity<RestaurantIngredient>(b =>
            {
                b.HasKey(x => new { x.RestaurantId, x.IngredientId });

                b.HasOne(x => x.Restaurant)
                    .WithMany(x => x.Ingredients)
                    .HasForeignKey(x => x.RestaurantId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasOne(x => x.Ingredient)
                    .WithMany(x => x.RestaurantIngredients)
                    .HasForeignKey(x => x.IngredientId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            builder.Entity<Supplier>(b =>
			{
                b.HasOne(x => x.Address)
                    .WithMany(x => x.Suppliers)
                    .HasForeignKey(x => x.AddressId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);


                b.HasKey(x => x.Id);
			});

            builder.Entity<SupplierIngredient>(b =>
            {
                b.HasKey(x => new { x.SupplierId, x.IngredientId });

                b.HasOne(x => x.Supplier)
                    .WithMany(x => x.Ingredients)
                    .HasForeignKey(x => x.SupplierId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasOne(x => x.Ingredient)
                    .WithMany(x => x.SupplierIngredients)
                    .HasForeignKey(x => x.IngredientId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            builder.Entity<SupplierOrder>(b =>
			{
                b.HasKey(x => x.Id);

                b.HasOne(x => x.Supplier)
                    .WithMany(x => x.SupplierOrders)
                    .HasForeignKey(x => x.SupplierId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasOne(x => x.Restaurant)
                    .WithMany(x => x.SupplierOrders)
                    .HasForeignKey(x => x.RestaurantId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);
			});

            builder.Entity<Table>(b =>
            {
                b.HasOne(x => x.Restaurant)
                    .WithMany(x => x.Tables)
                    .HasForeignKey(x => x.RestaurantId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasKey(x => x.Id);
            });
        }
    }
}
