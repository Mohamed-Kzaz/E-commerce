using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest() 
        {
            var product = _dbContext.Products.Find(100);
            if(product is null)
                return NotFound(new ApiResponse(400));

            return Ok(product);
        }

        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var product = _dbContext.Products.Find(100);

            // Will Throw Exception [NullReferenceException]
            var productToReturn = product.ToString();
            

            return Ok(productToReturn);
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }

        [HttpGet("badrequest/{id}")]
        public ActionResult GetBadRequest(int id) // Validation Error
        {
            return Ok();
        }

    }
}
