namespace Wasl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        //GetAll
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Supplier,StoreOwner")]

        public async Task<IActionResult> GetProducts()
        {
            var result = await productService.GetAllProduct();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //GetProductById
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Supplier, StoreOwner")]

        public async Task<IActionResult> GetProduct(Guid id)
        {
            var result = await productService.GetProductAsync(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Add Product
        [HttpPost("addProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Supplier")]

        public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
        {
            var result = await productService.AddProductAsync(addProductDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Update Product
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Supplier")]

        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {

            var result = await productService.UpdateProductAsync(updateProductDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //DeleteProduct
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Supplier")]

        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await productService.DeleteProductAsync(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
