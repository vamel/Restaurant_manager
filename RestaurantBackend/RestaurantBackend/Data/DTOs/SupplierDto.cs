using RestaurantBackend.Models;
using System.Collections.Generic;

namespace RestaurantBackend.Data.DTOs
{
	public class SupplierDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string BankInformation { get; set; }
		public int AddressId { get; set; }
		public AddressDto Address { get; set; }
		public IEnumerable<SupplierIngredientDto> Ingredients { get; set; }

		public SupplierDto() { }

		public SupplierDto(Supplier supplier)
		{
			Id = supplier.Id;
			Name = supplier.Name;
			BankInformation = supplier.BankInformation;
			AddressId = supplier.AddressId;
			Address = new AddressDto(supplier.Address);

			var l = new List<SupplierIngredientDto>();

			if (supplier.Ingredients != null)
			{
				foreach (var item in supplier.Ingredients)
				{
					l.Add(new SupplierIngredientDto
					{
						IngredientId = item.IngredientId,
						SupplierId = item.SupplierId,
					});
				}
			}

			Ingredients = l;
		}
	}
}
