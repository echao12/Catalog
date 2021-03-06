using Microsoft.AspNetCore.Mvc; //to inherit controller base
using Catalog.Repositories; //for InMemItemsRepository
using Catalog.Entities; //for Item
using System.Collections.Generic; //for IEnumerable
using System; //for Guid
using System.Linq;
using Catalog.Dtos;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers {
    [ApiController] //brings in some default behavior to make life easier
    [Route("items")]//what http this controller is reponding to
    public class ItemsController : ControllerBase {
        //private readonly InMemItemsRepository repository; //this was for a hard depedency on an instance
        
        // repository is now an interface reference variable. it can refer to any object that implements its interface.
        // note: interface vars can only access the methods declared in the interface, not the other methods in the class that implements the interface.
        private readonly IItemsRepository repository; // for dependency injection.
        private readonly ILogger<ItemsController> logger;

        //constructor
        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger) {
            //this causes a new instance of the repo each time an instance is made
            //thus a new database of items with new Guids will be generated each time
            //as a result, the Guids will always be outdated/invalid when we search with Get requests
            this.repository = repository; //the act of dependency injection
            this.logger = logger;
        }

        [HttpGet] // GetITems reacts to a GET with route /items
        public async Task<IEnumerable<ItemDto>> GetItemsAsync() {
            //project item into a ItemDto using Linq
            //note: wrap await with the async call to make the call chain work
            var items = (await repository.GetItemsAsync()).Select(item => item.AsDto());
            
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {items.Count()} items");
            return items;
        }

        [HttpGet("{id}")] // template for this route. Get /items/id
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id) { //ActionResult allows us to return different types
            var item = await repository.GetItemAsync(id);
            if (item is null) {
                return NotFound(); //ActionResult allows this return type
            }
            return item.AsDto();
        }

        //invoked from POST  /items  route
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto) {
            // generate Item object on the server side
            Item item = new() { // recall: C# 9 shorthand syntax. instead of new Item() 
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            await repository.CreateItemAsync(item); //invoke repository method to add item

            // return info about the created item
            // using createdAtAction here.
            //  passing in the action used to get information about the item, which is GetItem for this one.
            //  then pass the id for the GetItem route to use
            //  lastly, actual object being returned.
            return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsDto());
        }

        // invoked by PUT  /items/{id}  route
        //convention of put is to return no content, thus just an ActionResult with no alternative type.
        [HttpPut("{id}") ] // route template takes a id
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto) {
            //to update, first find item
            var existingItem = await repository.GetItemAsync(id);
            //check for valid existing item
            if(existingItem is null){
                return NotFound();
            }
            //update value
            //note: with-expressions create a copy of the object with specified modifications
            //  with-expressions are a property of Record types.
            Item updatedItem = existingItem with { 
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            await repository.UpdateItemAsync(updatedItem);

            return NoContent();
        }

        // invoked by DELETE /Item/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id) {
            var existingItem = await repository.GetItemAsync(id);

            if(existingItem is null){
                return NotFound();
            }
            
            await repository.DeleteItemAsync(id);

            return NoContent();
        }
    }
}