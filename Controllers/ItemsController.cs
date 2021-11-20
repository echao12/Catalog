using Microsoft.AspNetCore.Mvc; //to inherit controller base
using Catalog.Repositories; //for InMemItemsRepository
using Catalog.Entities; //for Item
using System.Collections.Generic; //for IEnumerable
using System; //for Guid


namespace Catalog.Controllers {
    [ApiController] //brings in some default behavior to make life easier
    [Route("items")]//what http this controller is reponding to
    public class ItemsController : ControllerBase {
        private readonly InMemItemsRepository repository;
        
        //constructor
        public ItemsController() {
            //this causes a new instance of the repo each time an instance is made
            //thus a new database of items with new Guids will be generated each time
            //as a result, the Guids will always be outdated/invalid when we search with Get requests
            repository = new InMemItemsRepository(); 
        }
        [HttpGet] // GetITems reacts to a GET with route /items
        public IEnumerable<Item> GetItems() {
            var items = repository.GetItems();
            return items;
        }

        [HttpGet("{id}")] // template for this route. Get /items/id
        public ActionResult<Item> GetItem(Guid id) { //ActionResult allows us to return different types
            var item = repository.GetItem(id);
            if (item is null) {
                return NotFound(); //ActionResult allows this return type
            }
            return item;
        }
    }
}