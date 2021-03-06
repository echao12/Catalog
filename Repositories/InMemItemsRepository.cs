using System;
using System.Collections.Generic;
using Catalog.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Repositories {


    // for dependency injection, we want to implement the dependency inversion principle.
    // We want the controller to depend on an interface, not a hard instance.
    // this way it dosen't care if its dependent on DependencyA or DependnecyB,
    //  it only cares that Dependency A/B implements the interface it depends on.
    // allows for easier modifications and cleaner code.
    public class InMemItemsRepository : IItemsRepository
    {
        private readonly List<Item> items = new()
        {
            new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 18, CreatedDate = DateTimeOffset.UtcNow },
        };

        //get all items
        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            //did not call async fn, just return a completed task.
            return await Task.FromResult(items);//return a completed task with this data.
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            //Where() returns a collection. use SingleOrDefault() to get the single element.
            //returns null if not found.
            var item = items.Where(item => item.Id == id).SingleOrDefault();
            return await Task.FromResult(item);
        }

        public async Task CreateItemAsync(Item item){
            items.Add(item); //Add is a list type method
            //don't have anything to return. just return a empty completed task.
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item item) {
            //find index where the existingItem matches item.Id
            var index = items.FindIndex(existingItem => existingItem.Id == item.Id);
            items[index] = item;
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Guid id) {
            var index = items.FindIndex(existingItem => existingItem.Id == id);
            items.RemoveAt(index);
            await Task.CompletedTask;
        }
    }
}