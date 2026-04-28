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
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderStatus? status = null)
        {
            var result = await supplierService.GetSupplierOrdersAsync(status);
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

        // Supplier submits a price offer for a pending order (عرض السعر).
        [HttpPut("orders/{orderId}/price-offer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitPriceOffer(Guid orderId, [FromBody] SubmitPriceOfferDto dto)
        {
            var result = await supplierService.SubmitPriceOfferAsync(orderId, dto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Supplier accepts a pending order.
        [HttpPut("orders/{orderId}/accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AcceptOrder(Guid orderId)
        {
            var result = await supplierService.AcceptOrderAsync(orderId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Supplier rejects a pending order.
        [HttpPut("orders/{orderId}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RejectOrder(Guid orderId)
        {
            var result = await supplierService.RejectOrderAsync(orderId);
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
