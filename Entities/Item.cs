using System;

namespace Catalog.Entities
{
    //record types are new to .net 5 & C# 9
    // used for immutable objects, 
    // supports with-expressions
    // supports value-based equality
    public record Item
    {
        // init is new. 
        // used for properties that only want to be set during initialization.
        public Guid Id {get; init;} //instead of get/set. set allows us to modify whenever.
        public string Name {get; init;}
        public decimal Price {get; init;}
        public DateTimeOffset CreatedDate{get; init;}

    }
}