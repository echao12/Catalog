using System;
using System.Collections.Generic;
using Catalog.Entities;

//the interface that the items controller depends on for 
// the dependency inversion principle.

namespace Catalog.Repositories {
    public interface IItemsRepository
    {
        Item GetItem(Guid id);
        IEnumerable<Item> GetItems();

        void CreateItem(Item item);
        void UpdateItem(Item item);
    }
}