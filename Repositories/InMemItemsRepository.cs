using System;
using System.Collections.Generic;
using Catalog.Entities;
using System.Linq;

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
        public IEnumerable<Item> GetItems()
        {
            return items;
        }

        public Item GetItem(Guid id)
        {
            //Where() returns a collection. use SingleOrDefault() to get the single element.
            //returns null if not found.
            return items.Where(item => item.Id == id).SingleOrDefault();
        }

        public void CreateItem(Item item){
            items.Add(item); //Add is a list type method
        }
    }
}