namespace Wasl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController(IAdminService adminService) : ControllerBase
    {

        // Get all users.
        [HttpGet("getusers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllUsers()
        {
            var result = await adminService.GetAllUserAsync();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Soft-delete a user.
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            var result = await adminService.DeleteUserAsync(userId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get all Suppliers pending approval.
        [HttpGet("suppliers/pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetPendingSuppliers()
        {
            var result = await adminService.GetPendingSuppliersAsync();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Approve a Supplier.
        [HttpPut("suppliers/{supplierId}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ApproveSupplier(Guid supplierId)
        {
            var result = await adminService.ApproveSupplierAsync(supplierId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Reject a Supplier.
        [HttpPut("suppliers/{supplierId}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RejectSupplier(Guid supplierId)
        {
            var result = await adminService.RejectSupplierAsync(supplierId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get all orders in the system for monitoring.
        [HttpGet("orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllOrders()
        {
            var result = await adminService.GetAllOrdersAsync();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }

}
