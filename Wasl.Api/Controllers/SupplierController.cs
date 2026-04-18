namespace Wasl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Supplier")]
    public class SupplierController(ISupplierService supplierService, ISupplierBrowseService supplierBrowseService) : ControllerBase
    {
        // Get supplier dashboard stats (product count, order counts, recent orders).
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await supplierBrowseService.GetSupplierDashboardAsync();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get all orders assigned to this Supplier.
        [HttpGet("orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await supplierService.GetSupplierOrdersAsync();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get pending orders for this Supplier.
        [HttpGet("orders/pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPendingOrders()
        {


            var result = await supplierService.GetPendingOrdersAsync();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Update order status
        [HttpPut("orders/{orderId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusDto dto)
        {

            var result = await supplierService.UpdateOrderStatusAsync(orderId, dto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Update tracking info.
        [HttpPut("orders/{orderId}/tracking")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTracking(Guid orderId, [FromBody] UpdateTrackingDto dto)
        {

            var result = await supplierService.UpdateTrackingAsync(orderId, dto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
