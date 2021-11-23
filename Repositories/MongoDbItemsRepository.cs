using System;
using System.Collections.Generic;
using Catalog.Entities;
using MongoDB.Driver; //allows us to use mongoDB

namespace Catalog.Repositories {
    public class MongoDbItemsRepository : IItemsRepository
    {
        
        private const string databaseName = "catalog"; 
        private const string collectionName = "items"; 
        //collection is how mongoDB associates all the entities together
        private readonly IMongoCollection<Item> itemsCollection;//collection of Items. readonly b/c not ganna change after initialization.
        //constructor to inject the database info. takes a MongoDB Client as parameter.
        public MongoDbItemsRepository(IMongoClient mongoClient) {
            //fetch database with mongoClient
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            //fetch an "Item" collection from database with collectionName.
            itemsCollection = database.GetCollection<Item>(collectionName);
            //note: database and collection will be created when its needed. if it dne yet, it will create it for us.
        }
        public void CreateItem(Item item)
        {
            itemsCollection.InsertOne(item);
        }

        public void DeleteItem(Guid item)
        {
            throw new NotImplementedException();
        }

        public Item GetItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> GetItems()
        {
            throw new NotImplementedException();
        }

        public void UpdateItem(Item item)
        {
            throw new NotImplementedException();
        }
    }
}