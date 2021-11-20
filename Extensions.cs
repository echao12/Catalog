using Catalog.Dtos;
using Catalog.Entities;

namespace Catalog {
    // Extensions must be a static class.
    //they just extend the definition of a type by adding methods that this class can execute.
    public static class Extensions {
        //AsDto operates on the current item. hence (this Item item) parameter
        public static ItemDto AsDto(this Item item){
            return new ItemDto {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                CreatedDate = item.CreatedDate
            };
        }
    }
}