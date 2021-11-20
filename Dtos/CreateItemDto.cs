using System.ComponentModel.DataAnnotations; // enables [Required] & [Range()] annotation

namespace Catalog.Dtos {
    public record CreateItemDto {
        //note: Guid & CretedTime should be generated on the server side

        //client should pass name and price
        [Required] //must be provided
        public string Name {get; init;} 
        [Required]
        [Range(1,1000)] // restrict valid values to be b/t [1 - 1000]
        public decimal Price {get; init;}
    }
}