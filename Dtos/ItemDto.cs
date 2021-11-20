//Dto -> Data Transfer Object
// this is a contract from client and server.
//we will only be exposing these properties to the client 
//instead of the entire raw item entity.

using System;
//just so happens that this Dto currently exposes all properties of Items.
//power of Dto more apparent when we expand Items in future updates.
namespace Catalog.Dtos {
    public record ItemDto {
        public Guid Id {get; init;} 
        public string Name {get; init;}
        public decimal Price {get; init;}
        public DateTimeOffset CreatedDate{get; init;}
    }
}