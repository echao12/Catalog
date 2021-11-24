using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;

//the interface that the items controller depends on for 
// the dependency inversion principle.

//note: VS Code can rename all instances of var with f2/right click rename symbol
// ctr + . => add namespace
namespace Catalog.Repositories {
    public interface IItemsRepository
    {
        //Task is for async fns. Task wraps Datatype and tells caller it will eventually return <type>
        Task<Item> GetItemAsync(Guid id);
        Task<IEnumerable<Item>> GetItemsAsync();

        Task CreateItemAsync(Item item); //void fns just return Task.
        Task UpdateItemAsync(Item item);

        Task DeleteItemAsync(Guid item);
    }
}