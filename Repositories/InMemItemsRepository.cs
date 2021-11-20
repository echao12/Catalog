using System;
using System.Collections.Generic;
using Catalog.Entities;
using System.Linq;

namespace Catalog.Repositories {
    public class InMemItemsRepository
    {
        private readonly List<Item> items = new() 
        {
            new Item {Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow },
            new Item {Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreatedDate = DateTimeOffset.UtcNow },
            new Item {Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 18, CreatedDate = DateTimeOffset.UtcNow },
        };

        //get all items
        public IEnumerable<Item> GetItems() {
            return items;
        }

        public Item GetItem(Guid id) {
            //Where() returns a collection. use SingleOrDefault() to get the single element.
            //returns null if not found.
            return items.Where(item => item.Id == id).SingleOrDefault();
        }
    }
}