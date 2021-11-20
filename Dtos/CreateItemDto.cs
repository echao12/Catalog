namespace Catalog.Dtos {
    public record CreateItemDto {
        //note: Guid & CretedTime should be generated on the server side

        //client should pass name and price
        public string Name {get; init;} 
        public decimal Price {get; init;}
    }
}