namespace Wasl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SuppliersController(ISupplierBrowseService supplierBrowseService) : ControllerBase
    {
        // Get all approved suppliers. Supports optional search query.

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetApprovedSuppliers([FromQuery] string? search = null)
        {
            var result = await supplierBrowseService.GetApprovedSuppliersAsync(search);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get supplier details by ID.

        [HttpGet("{supplierId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSupplierById(Guid supplierId)
        {
            var result = await supplierBrowseService.GetSupplierByIdAsync(supplierId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get supplier products by supplier ID.

        [HttpGet("{supplierId:guid}/products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSupplierProducts(Guid supplierId)
        {
            var result = await supplierBrowseService.GetSupplierProductsAsync(supplierId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
