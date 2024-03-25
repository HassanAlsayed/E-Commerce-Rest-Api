using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebStoreApi.DTO;
using WebStoreApi.Repository;

namespace WebStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository _products;

        public ProductsController(IProductsRepository products)
        {
            _products = products;
        }
       
        [HttpGet]
        public async Task<ActionResult> GetAllProducts(int? page,string? search) {

          return Ok(await _products.GetAllProduct(page,search));
         }
       
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById(Guid id)
        {

            return Ok(await _products.GetProductById(id));
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTO product)
        {
            var createProduct = await _products.CreateProduct(product);
            return CreatedAtAction(nameof(GetProductById),new {id = createProduct.Id},createProduct);
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var DeletedProduct = await _products.DeleteProduct(id);
            return Ok(DeletedProduct);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductDTO product ,Guid id)
        {
            return Ok(await _products.UpdateProduct(product, id));
        }
    }
}
