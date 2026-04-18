namespace Wasl.Application.Interfaces
{
    public interface IAdminService
    {
        // User Management
        Task<ResultResponse<UserProfileResponse>> GetAllUserAsync();
        Task<ResultResponse<UserProfileResponse>> DeleteUserAsync(Guid userId);

        // Supplier Approval
        Task<ResultResponse<UserProfileResponse>> GetPendingSuppliersAsync();
        Task<ResultResponse<UserProfileResponse>> ApproveSupplierAsync(Guid supplierId);
        Task<ResultResponse<UserProfileResponse>> RejectSupplierAsync(Guid supplierId);

        // Order Monitoring
        Task<ResultResponse<OrderResponse>> GetAllOrdersAsync();
    }
}
