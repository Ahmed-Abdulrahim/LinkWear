namespace Wasl.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        // POST /api/orders — StoreOwner creates a new order, selecting a Supplier.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
        {
            var result = await orderService.CreateOrderAsync(createOrderDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // StoreOwner cancels a pending order.
        [HttpPut("cancelOrder/{id}")]
        [Authorize(Roles = "StoreOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var result = await orderService.CancelOrderAsync(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // StoreOwner gets all their orders.
        [HttpGet]
        [Authorize(Roles = "StoreOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrders()
        {
            var result = await orderService.GetOrdersAsync();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get order details (StoreOwner or Supplier).
        [HttpGet("{id}")]
        [Authorize(Roles = "StoreOwner,Supplier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            var result = await orderService.GetOrderDetails(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
