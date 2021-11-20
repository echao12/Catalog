using Microsoft.AspNetCore.Mvc; //to inherit controller base
using Catalog.Repositories; //for InMemItemsRepository
using Catalog.Entities; //for Item
using System.Collections.Generic; //for IEnumerable
using System; //for Guid
using System.Linq;
using Catalog.Dtos;


namespace Catalog.Controllers {
    [ApiController] //brings in some default behavior to make life easier
    [Route("items")]//what http this controller is reponding to
    public class ItemsController : ControllerBase {
        //private readonly InMemItemsRepository repository; //this was for a hard depedency on an instance
        
        // repository is now an interface reference variable. it can refer to any object that implements its interface.
        // note: interface vars can only access the methods declared in the interface, not the other methods in the class that implements the interface.
        private readonly IItemsRepository repository; // for dependency injection.

        //constructor
        public ItemsController(IItemsRepository repository) {
            //this causes a new instance of the repo each time an instance is made
            //thus a new database of items with new Guids will be generated each time
            //as a result, the Guids will always be outdated/invalid when we search with Get requests
            this.repository = repository; //the act of dependency injection
        }

        [HttpGet] // GetITems reacts to a GET with route /items
        public IEnumerable<ItemDto> GetItems() {
            //project item into a ItemDto using Linq
            var items = repository.GetItems().Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")] // template for this route. Get /items/id
        public ActionResult<ItemDto> GetItem(Guid id) { //ActionResult allows us to return different types
            var item = repository.GetItem(id);
            if (item is null) {
                return NotFound(); //ActionResult allows this return type
            }
            return item.AsDto();
        }

        //invoked from POST  /items  route
        [HttpPost]
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto) {
            // generate Item object on the server side
            Item item = new() { // recall: C# 9 shorthand syntax. instead of new Item() 
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            repository.CreateItem(item); //invoke repository method to add item

            // return info about the created item
            // using createdAtAction here.
            //  passing in the action used to get information about the item, which is GetItem for this one.
            //  then pass the id for the GetItem route to use
            //  lastly, actual object being returned.
            return CreatedAtAction(nameof(GetItem), new {id = item.Id}, item.AsDto());
        }
    }
}