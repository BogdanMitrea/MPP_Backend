using Microsoft.AspNetCore.Mvc;
using MPP_BackEnd.Repositories;
using MPP_BackEnd;
using MPP_Backend.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace MPP_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IRepositoryStore _repositorystore;
        public StoreController(IRepositoryStore repositorystore)
        {
            this._repositorystore = repositorystore;

        }

        [HttpGet]
        public IEnumerable<Store> GetAllStores()
        {
            return this._repositorystore.GetAllStores();
        }

        [HttpGet("{id}")]
        public IActionResult GetStoreById(int id)
        {
            var storeModel = this._repositorystore.GetAllStores().FirstOrDefault(p => p.Id == id);
            if (storeModel == null)
            {
                return NotFound(); // Return 404 Not Found if store with given id is not found
            }
            return Ok(storeModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddNewStore([FromBody] Store store)
        {
            this._repositorystore.AddStore(store);
            return Ok(store); // Returns the added store object as JSON
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteStore(int id)
        {
            if (this._repositorystore.DeleteStore(id))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Updatestore(int id, [FromBody] Store newmodel)
        {
            if (this._repositorystore.UpdateStore(id, newmodel))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
