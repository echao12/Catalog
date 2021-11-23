using System;
using System.Collections.Generic;
using Catalog.Entities;
using MongoDB.Driver; //allows us to use mongoDB
using MongoDB.Bson;

namespace Catalog.Repositories {
    public class MongoDbItemsRepository : IItemsRepository
    {
        
        private const string databaseName = "catalog"; 
        private const string collectionName = "items"; 
        //collection is how mongoDB associates all the entities together
        private readonly IMongoCollection<Item> itemsCollection;//collection of Items. readonly b/c not ganna change after initialization.
        //used to filter the items returned when we find them in the collection.
        //add as a class variable b/c we will be using it often.
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter; //reference to filter object

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
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item);
            itemsCollection.DeleteOne(filter); //use filter to match Guids and call DeleteOne to delete.
        }

        public Item GetItem(Guid id)
        {
            //create the filter with the builder
            var filter = filterBuilder.Eq(item => item.Id, id); // filter items where their id matches the passed id
            //look through collection and use the filter to find only 1 instance of the item.
            return itemsCollection.Find(filter).SingleOrDefault();
        }

        public IEnumerable<Item> GetItems()
        {
            //many ways to fetch all items. this is one.
            return itemsCollection.Find(new BsonDocument()).ToList(); //get all items in the colection.
        }

        public void UpdateItem(Item item)
        {
            //search for the item with filters
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            itemsCollection.ReplaceOne(filter, item);//replace the item found by filter with item argument.
        }
    }
}