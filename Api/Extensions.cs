using Catalog.Dtos;
using Catalog.Entities;

namespace Catalog
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new() { Id = item.Id, Name = item.Name, Price = item.Price, CreatedDate = item.CreatedDate };
        }
    }
}
